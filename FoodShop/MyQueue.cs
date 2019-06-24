using System;
using System.Threading;

namespace FoodShop
{
    public class MyQueue<T>// where T:Buyer
    {
        private volatile Segment m_head;
        private volatile Segment m_tail;

        private const int SEGMENT_SIZE = 32;

        internal volatile int m_numSnapshotTakers = 0;

        public MyQueue()
        {
            m_head = m_tail = new Segment(0, this);
        }

        public int Count
        {
            get
            {
                Segment head, tail;
                int headLow, tailHigh;
                GetHeadTailPositions(out head, out tail, out headLow, out tailHigh);

                if (head == tail)
                {
                    return tailHigh - headLow + 1;
                }

                int count = SEGMENT_SIZE - headLow;

                count += SEGMENT_SIZE * ((int)(tail.m_index - head.m_index - 1));

                count += tailHigh + 1;

                return count;
            }
        }

        public void Enqueue(T item)
        {
            if (item == null)
            {
                Console.WriteLine("RIP buyer");
                return;
            }
            SpinWait spin = new SpinWait();
            while (true)
            {
                Segment tail = m_tail;
                if (tail.TryAppend(item))
                    return;
                spin.SpinOnce();
            }
        }

        public bool TryDequeue(out T result)
        {
            while (!IsEmpty)
            {
                Segment head = m_head;
                if (head.TryRemove(out result))
                {
                    if (result == null) Console.WriteLine("Try Dequeue Null");
                    return true;
                }
                //since method IsEmpty spins, we don't need to spin in the while loop
            }
            result = default(T);
            return false;
        }

        public bool IsEmpty
        {
            get
            {
                Segment head = m_head;
                if (!head.IsEmpty)
                    return false;
                else if (head.Next == null)
                    return true;
                else
                {
                    SpinWait spin = new SpinWait();
                    while (head.IsEmpty)
                    {
                        if (head.Next == null)
                            return true;

                        spin.SpinOnce();
                        head = m_head;
                    }
                    return false;
                }
            }
        }

        public bool TryPeek(out T result)
        {
            Interlocked.Increment(ref m_numSnapshotTakers);

            while (!IsEmpty)
            {
                Segment head = m_head;
                if (head.TryPeek(out result))
                {
                    Interlocked.Decrement(ref m_numSnapshotTakers);
                    return true;
                }
                //since method IsEmpty spins, we don't need to spin in the while loop
            }
            result = default(T);
            Interlocked.Decrement(ref m_numSnapshotTakers);
            return false;
        }

        private void GetHeadTailPositions(out Segment head, out Segment tail,
            out int headLow, out int tailHigh)
        {
            head = m_head;
            tail = m_tail;
            headLow = head.Low;
            tailHigh = tail.High;
            SpinWait spin = new SpinWait();

            while (head != m_head || tail != m_tail
                || headLow != head.Low || tailHigh != tail.High
                || head.m_index > tail.m_index)
            {
                spin.SpinOnce();
                head = m_head;
                tail = m_tail;
                headLow = head.Low;
                tailHigh = tail.High;
            }
        }

        private class Segment
        {
            internal volatile T[] m_array;
            internal volatile VolatileBool[] m_state;
            private volatile Segment m_next;
            internal readonly long m_index;

            private volatile int m_low;
            private volatile int m_high;

            private volatile MyQueue<T> m_source;

            internal Segment(long index, MyQueue<T> source)
            {
                m_array = new T[SEGMENT_SIZE];
                m_state = new VolatileBool[SEGMENT_SIZE];
                m_high = -1;
                m_index = index;
                m_source = source;
            }

            internal Segment Next
            {
                get { return m_next; }
            }

            internal bool IsEmpty
            {
                get { return (Low > High); }
            }

            internal void UnsafeAdd(T value)
            {
                m_high++;
                m_array[m_high] = value;
                m_state[m_high].m_value = true;
            }

            internal Segment UnsafeGrow()
            {
                Segment newSegment = new Segment(m_index + 1, m_source); 
                m_next = newSegment;
                return newSegment;
            }

            internal void Grow()
            {
                Segment newSegment = new Segment(m_index + 1, m_source);
                m_next = newSegment;
                m_source.m_tail = m_next;
            }

            internal bool TryAppend(T value)
            {
                if (m_high >= SEGMENT_SIZE - 1)
                {
                    return false;
                }


                int newhigh = SEGMENT_SIZE;

                newhigh = Interlocked.Increment(ref m_high);
                if (newhigh <= SEGMENT_SIZE - 1)
                {
                    m_array[newhigh] = value;
                    m_state[newhigh].m_value = true;
                }

                if (newhigh == SEGMENT_SIZE - 1)
                {
                    Grow();
                }

                return newhigh <= SEGMENT_SIZE - 1;
            }

            internal bool TryRemove(out T result)
            {
                SpinWait spin = new SpinWait();
                int lowLocal = Low, highLocal = High;
                while (lowLocal <= highLocal)
                {
                    if (Interlocked.CompareExchange(ref m_low, lowLocal + 1, lowLocal) == lowLocal)
                    {
                        SpinWait spinLocal = new SpinWait();
                        while (!m_state[lowLocal].m_value)
                        {
                            spinLocal.SpinOnce();
                        }
                        result = m_array[lowLocal];

                        if (m_source.m_numSnapshotTakers <= 0)
                        {
                            m_array[lowLocal] = default(T); 
                        }

                        if (lowLocal + 1 >= SEGMENT_SIZE)
                        {
                            
                            spinLocal = new SpinWait();
                            while (m_next == null)
                            {
                                spinLocal.SpinOnce();
                            }
                            m_source.m_head = m_next;
                        }
                        return true;
                    }
                    else
                    {
                        
                        spin.SpinOnce();
                        lowLocal = Low; highLocal = High;
                    }
                }
                result = default(T);
                return false;
            }

            internal bool TryPeek(out T result)
            {
                result = default(T);
                int lowLocal = Low;
                if (lowLocal > High)
                    return false;
                SpinWait spin = new SpinWait();
                while (!m_state[lowLocal].m_value)
                {
                    spin.SpinOnce();
                }
                result = m_array[lowLocal];
                return true;
            }

            internal void AddToList(MyList<T> list, int start, int end)
            {
                for (int i = start; i <= end; i++)
                {
                    SpinWait spin = new SpinWait();
                    while (!m_state[i].m_value)
                    {
                        spin.SpinOnce();
                    }
                    list.Add(m_array[i]);
                }
            }

            internal int Low
            {
                get
                {
                    return Math.Min(m_low, SEGMENT_SIZE);
                }
            }

            internal int High
            {
                get
                {
                    return Math.Min(m_high, SEGMENT_SIZE - 1);
                }
            }

        }

        public struct SpinWait
        {

            internal const int YIELD_THRESHOLD = 10; 
            internal const int SLEEP_0_EVERY_HOW_MANY_TIMES = 5; 
            internal const int SLEEP_1_EVERY_HOW_MANY_TIMES = 20;

            private int m_count;

            public int Count
            {
                get { return m_count; }
            }

            public bool NextSpinWillYield
            {
                get { return m_count > YIELD_THRESHOLD || Helper.IsSingleProcessor; }
            }

            public void SpinOnce()
            {
                if (NextSpinWillYield)
                {
                   
                    CdsSyncEtwBCLProvider.Log.SpinWait_NextSpinWillYield();

                    int yieldsSoFar = (m_count >= YIELD_THRESHOLD ? m_count - YIELD_THRESHOLD : m_count);

                    if ((yieldsSoFar % SLEEP_1_EVERY_HOW_MANY_TIMES) == (SLEEP_1_EVERY_HOW_MANY_TIMES - 1))
                    {
                        Thread.Sleep(1);
                    }
                    else if ((yieldsSoFar % SLEEP_0_EVERY_HOW_MANY_TIMES) == (SLEEP_0_EVERY_HOW_MANY_TIMES - 1))
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {

                        Thread.Yield();
                    }
                }
                else
                {
                   
                    Thread.SpinWait(4 << m_count);
                }

                m_count = (m_count == int.MaxValue ? YIELD_THRESHOLD : m_count + 1);
            }

            
            public void Reset()
            {
                m_count = 0;
            }

            
            public static void SpinUntil(Func<bool> condition)
            {
                SpinUntil(condition, Timeout.Infinite);
            }

           
            public static bool SpinUntil(Func<bool> condition, TimeSpan timeout)
            { 
                Int64 totalMilliseconds = (Int64)timeout.TotalMilliseconds;
                if (totalMilliseconds < -1 || totalMilliseconds > Int32.MaxValue)
                {
                    throw new System.ArgumentOutOfRangeException();
                }
                return SpinUntil(condition, (int)timeout.TotalMilliseconds);
            }

            public static bool SpinUntil(Func<bool> condition, int millisecondsTimeout)
            {
                if (millisecondsTimeout < Timeout.Infinite)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (condition == null)
                {
                    throw new ArgumentNullException();
                }
                uint startTime = 0;
                if (millisecondsTimeout != 0 && millisecondsTimeout != Timeout.Infinite)
                {
                    startTime = Helper.GetTime();
                }
                SpinWait spinner = new SpinWait();
                while (!condition())
                {
                    if (millisecondsTimeout == 0)
                    {
                        return false;
                    }

                    spinner.SpinOnce();

                    if (millisecondsTimeout != Timeout.Infinite && spinner.NextSpinWillYield)
                    {
                        if (millisecondsTimeout <= (Helper.GetTime() - startTime))
                        {
                            return false;
                        }
                    }
                }
                return true;

            }

        }

        struct VolatileBool
        {
            public VolatileBool(bool value)
            {
                m_value = value;
            }
            public volatile bool m_value;
        }

    }
}

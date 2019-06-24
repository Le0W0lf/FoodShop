using System;
using System.Diagnostics.Tracing;

namespace FoodShop
{
    internal static class Helper
    {
        internal const int MaxArrayLength = 0X7FEFFFFF;
        internal const int MaxByteArrayLength = 0x7FFFFFC7;
        static public Random rnd = new Random();

        public static MyList<Seller> NewListOfSellers(int capacity)
        {
            MyList<Seller> newSellers = new MyList<Seller>();
            for (int i = 0; i < capacity; i++)
            {
                newSellers.Add(new Seller());
            }
            return newSellers;
        }

        public static uint GetTime()
        {
            return (uint)Environment.TickCount;
        }


        private const int PROCESSOR_COUNT_REFRESH_INTERVAL_MS = 30000; // How often to refresh the count, in milliseconds.
        private static volatile int s_processorCount; // The last count seen.
        private static volatile int s_lastProcessorCountRefreshTicks; // The last time we refreshed.

        internal static int ProcessorCount
        {
            get
            {
                int now = Environment.TickCount;
                int procCount = s_processorCount;
                if (procCount == 0 || (now - s_lastProcessorCountRefreshTicks) >= PROCESSOR_COUNT_REFRESH_INTERVAL_MS)
                {
                    s_processorCount = procCount = Environment.ProcessorCount;
                    s_lastProcessorCountRefreshTicks = now;
                }


                return procCount;
            }
        }

        internal static bool IsSingleProcessor
        {
            get { return ProcessorCount == 1; }
        }

    }


    internal sealed class CdsSyncEtwBCLProvider : EventSource
    {
        public static CdsSyncEtwBCLProvider Log = new CdsSyncEtwBCLProvider();

        private CdsSyncEtwBCLProvider() { }

        private const EventKeywords ALL_KEYWORDS = (EventKeywords)(-1);


        private const int SPINWAIT_NEXTSPINWILLYIELD_ID = 2;

        public void SpinWait_NextSpinWillYield()
        {
            if (IsEnabled(EventLevel.Informational, ALL_KEYWORDS))
            {
                WriteEvent(SPINWAIT_NEXTSPINWILLYIELD_ID);
            }
        }


    }

}

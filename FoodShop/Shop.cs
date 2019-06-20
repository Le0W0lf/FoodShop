using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop
{
    class Shop
    {
        private Random rnd;
        public bool IsOpen { get; private set; }
        public int countOfStands { get; private set; }
        private MyList<Stand> stands;

        public Shop()
        {
            IsOpen = false;
            countOfStands = 3;
            stands = new MyList<Stand>();
            stands.Add(new Stand("ice cream", 9.99));
            stands.Add(new Stand("cake", 49.99));
            stands.Add(new Stand("chocolate", 17.99));
            rnd = new Random();
        }

        public void Open()
        {
            IsOpen = true;
            Console.WriteLine("Shop is open");
            Parallel.ForEach(stands, (x) =>
            {
                int countOfSellers = rnd.Next(3, 7);
                var sellers = Helper.NewListOfSellers(countOfSellers);
                foreach(var t in sellers)
                {
                    t.StandId += FindIndex;
                }
                x.Open(countOfSellers, sellers);
            });
        }

        public void Close()
        {
            if (!IsOpen)
            {
                Console.WriteLine("Shop already close");
                return;
            }
            Console.WriteLine("Shop is close");
            Parallel.ForEach(stands, (x) => x.Close());
            IsOpen = false;
        }

        private void ToQueue(Buyer buyer,int index)
        {
            if (!IsOpen)
            {
                Console.WriteLine("Oops, shop is closed :(");
                return;
            }

            //string s = "";
            //MyList<double> queue = new List<double>();

            double min = stands[index].CountInQueue;
            for (int i = index; i < countOfStands; i++) 
            {
                double curr = stands[i].CountInQueue;

                //queue.Add(curr);

                if (buyer.Stands[i]) continue;
                if (curr.Equals(0.0))
                {
                    index = i;
                    min = curr;
                    break;
                }
                if (curr < min)
                {
                    
                    index = i;
                    min = curr;
                }
            }
            stands[index].Buyers.Enqueue(buyer);
            /*foreach(var t in queue)
            {
                s+= (t<1?"0":"") + t.ToString("#.##")+" ";
            }
            Console.WriteLine($"{s}    Buyer go to queue for {stands[index].TypeFood} ({min:N2})");*/
        }

        private int FindIndex(Stand stand)
        {
            for (int i = 0; i < countOfStands; i++)
            {
                if (stand == stands[i]) return i;
            }
            return -1;
        }

        public void Add(Buyer buyer)
        {
            //Console.WriteLine("Add new buyer");
            buyer.ToQueue += ToQueue;
            buyer.CallToQueue();
        }

        public void Statistic()
        {
            string stats="";
            foreach(var stand in stands)
            {
                stats+=stand.Statistic();
            }
            Console.WriteLine($"####################### STATISTICS #######################\n{stats}##########################################################");
        }

        public void Start()
        {
            Parallel.ForEach(stands, (x) => x.Start());
        }
    }
}

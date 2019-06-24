using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodShop
{
    public class Shop
    {
        public bool IsOpen { get; private set; }
        public int countOfStands { get; private set; }
        private MyList<Stand> stands;
        public static int RIP=0;
        public Shop()
        {
            IsOpen = false;
            countOfStands = 3;
            stands = new MyList<Stand>();
            stands.Add(new Stand("ice cream", 9.99, Product.IceCream));
            stands.Add(new Stand("cake", 49.99, Product.Cake));
            stands.Add(new Stand("chocolate", 17.99, Product.Chocolate));
        }

        public void Open()
        {
            if (IsOpen)
            {
                Console.WriteLine("The shop is already open");
                return;
            }
            IsOpen = true;
            Console.WriteLine("The shop is open");
            Parallel.ForEach(stands, (x) =>
            {
                int countOfSellers = Helper.rnd.Next(3, 7);
                var sellers = Helper.NewListOfSellers(countOfSellers);
                x.Open(countOfSellers, sellers);
            });
            Start();
        }

        public void Close()
        {
            if (!IsOpen)
            {
                Console.WriteLine("The shop is already close");
                return;
            }
            Console.WriteLine("The shop is close");
            Parallel.ForEach(stands, (x) => x.Close());
            IsOpen = false;
        }
        //Method for buyer`s event 
        private void ToQueue(Buyer buyer)
        {
            if (!IsOpen)
            {
                Console.WriteLine("Oops, shop is closed :(");
                return;
            }

            if (buyer == null) Console.WriteLine("REALY??????????");
            //string s = "";
            //MyList<double> queue = new MyList<double>();
            
            double min = int.MaxValue;
            int index = -1;
            for (int i = 0; i < countOfStands; i++) 
            {
                double curr = stands[i].CountInQueue;

                //queue.Add(curr);

                if ((buyer.Products&stands[i].StandProduct)!=0) continue;
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
            if (index != -1) stands[index].Buyers.Enqueue(buyer);
            else Interlocked.Increment(ref RIP);
            //else Console.WriteLine(buyer.Products);
            /*if (index == -1) { Console.WriteLine("Buyer RIP"); return; }
            foreach(var t in queue)
            {
                s+= (t<1?"0":"") + t.ToString("#.##")+" ";
            }
            Console.WriteLine($"{s}    Buyer go to queue for {stands[index].TypeFood} ({min:N2})");*/
        }
        //Find index for current stand
        private int FindIndex(Stand stand)
        {
            for (int i = 0; i < countOfStands; i++)
            {
                if (stand == stands[i]) return i;
            }
            return -1;
        }
        //Add new buyer
        public void Add(Buyer buyer)
        {
            //Console.WriteLine("Add new buyer");
            buyer.ToQueue += ToQueue;
            buyer.CallToQueue();
        }
        //See statistic
        public void Statistic()
        {
            string stats=$"RIP:{RIP} buyers\n";
            foreach(var stand in stands)
            {
                stats+=stand.Statistic();
            }
            Console.WriteLine($"####################### STATISTICS #######################\n{stats}##########################################################");
        }
        //Start sending
        public void Start()
        {
            if(IsOpen)
                Parallel.ForEach(stands, (x) => x.Start());
            else
                Console.WriteLine("The shop is close");
        }
    }
}

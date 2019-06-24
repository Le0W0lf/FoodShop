using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodShop
{
    class User
    {
        private Shop shop;
        private int x;
        private double y;
        private bool add = false;

        public User()
        {
            shop = new Shop();
        }

        public void Open()
        {
            shop.Open();
        }

        public void Close()
        {
            shop.Close();
            add = false;
        }

        public void Start(int x, double y)
        {
            if (!shop.IsOpen)
            {
                Console.WriteLine("The shop is close");
                return;
            }
            this.x = x;
            this.y = y;
            if (x <= 0 || y <= 0) 
            {
                Console.WriteLine("X and Y must be greater than 0");
                return;
            }
            
            //Console.WriteLine("We advertise the store for new buyers, please wait");
            //Bug was fixed
            if (!add)
            {
                add = true;
                Task.Run(() => Start());
            }
            //Console.WriteLine("Start2");
        }

        public void Statistic()
        {
            shop.Statistic();
        }

        private void Start()
        {
            shop.Start();
            Console.WriteLine("Start adding");
            while(add)
            {
                //Thread.Sleep(Convert.ToInt32(y * 1000));
                if (!add) return;
                Console.WriteLine($"Add {x} new buyers");
                for (int i = 0; i < x; i++) 
                {
                    shop.Add(new Buyer());
                }
                Thread.Sleep(Convert.ToInt32(y * 1000));
            }
        }

        internal void Stop()
        {
            add = false;
            Console.WriteLine("Stop adding");
        }
    }
}

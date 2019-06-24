using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FoodShop
{
    public class Stand
    {
        public string TypeFood { get; private set; }
        public int CountOfSellers { get; private set; }
        public int Selled;// { get; private set; }
        public double Price { get; private set; }
        public int RIPBuyers;// { get; private set; }
        MyList<Seller> sellers;
        public MyQueue<Buyer> Buyers;
        public bool IsOpen { get; private set; }
        public readonly Product StandProduct;
        

        public Stand(string typefood, double price, Product product)
        {
            TypeFood = typefood;
            Selled = 0;
            RIPBuyers = 0;
            Price = price;
            StandProduct = product;
        }
        //When shop is open
        public void Open(int count,MyList<Seller> sellers)
        {
            Console.WriteLine($"Stand with {TypeFood} start working ({count} sellers)");
            IsOpen = true;
            this.sellers = sellers;
            CountOfSellers = count;
            Buyers = new MyQueue<Buyer>();
        }
        //When shop is close
        public void Close()
        {
            Console.WriteLine($"Stand with {TypeFood} stop working");
            IsOpen = false;
        }
        //For calculate minimum queue
        public double CountInQueue
        {
            get
            {
                return (double)Buyers.Count / CountOfSellers;
            }
        }
        //See statistic
        public string Statistic()
        {
            return $"{TypeFood}: was {RIPBuyers} buyers, buyed {Selled} foods (earn {Price*Selled}$)\n";
        }
        //When user start sending
        internal void Start()
        {

            //Parallel.ForEach(this.sellers, (x) => x.Start(this));
            foreach(var x in sellers)
            {
                x.Start(this);
            }
        }
    }
}

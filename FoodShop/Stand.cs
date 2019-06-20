using System;
using System.Threading.Tasks;

namespace FoodShop
{
    class Stand
    {
        public string TypeFood { get; private set; }
        public int CountOfSellers { get; private set; }
        public int Selled;// { get; private set; }
        public double Price { get; private set; }
        public int RIPBuyers;// { get; private set; }
        MyList<Seller> sellers;
        public MyQueue<Buyer> Buyers;
        public Random rnd;
        public bool IsOpen { get; private set; }

        public Stand(string typefood, double price)
        {
            TypeFood = typefood;
            Selled = 0;
            RIPBuyers = 0;
            Price = price;
            rnd = new Random();
        }

        public void Open(int count,MyList<Seller> sellers)
        {
            Console.WriteLine($"Stand with {TypeFood} start working ({count} sellers)");
            IsOpen = true;
            this.sellers = sellers;
            CountOfSellers = count;
            Buyers = new MyQueue<Buyer>();
        }

        public void Close()
        {
            Console.WriteLine($"Stand with {TypeFood} stop working");
            IsOpen = false;
        }

        public double CountInQueue
        {
            get
            {
                return (double)Buyers.Count / CountOfSellers;
            }
        }

        public string Statistic()
        {
            return $"{TypeFood}: was {RIPBuyers} buyers, buyed {Selled} foods (earn {Price*Selled}$)\n";
        }

        internal void Start()
        {

            Parallel.ForEach(this.sellers, (x) => x.Start(this));
        }
    }
}

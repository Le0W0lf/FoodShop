

namespace FoodShop
{
    public delegate void DelegateForBuyer(Buyer buyer);
    public class Buyer
    {
        public event DelegateForBuyer ToQueue;
        public Product Products { get; private set; }
        //public MyList<bool> Stands;

        public int Buy(Product product)
        {
            if ((Products & product) != 0) return 0;
            Products ^= product;
            return Helper.rnd.Next(1, 4);
        }

        //For call event from Seller
        public void CallToQueue()
        {
            if (ToQueue == null) System.Console.WriteLine("RIP event");
            ToQueue?.Invoke(this);
        }

    }
}

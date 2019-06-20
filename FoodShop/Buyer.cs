

namespace FoodShop
{
    delegate void DelegateForBuyer(Buyer buyer,int index);
    class Buyer
    {
        public event DelegateForBuyer ToQueue;
        public MyList<bool> Stands;
        public Buyer(int CountOfStands=3)
        {
            Stands = new MyList<bool>();
            for (int i = 0; i < CountOfStands; i++)
            {
                Stands.Add(false);
            }
        }

        public void CallToQueue()
        {
            int CountOfStands = Stands.Count;
            for (int i = 0; i < CountOfStands; i++)
            {
                if(!Stands[i])
                {
                    ToQueue?.Invoke(this,i);
                    break;
                }
            }
        }

    }
}

using System.Threading;
using System.Threading.Tasks;

namespace FoodShop
{
    delegate int DelegateForSeller(Stand stand);
    public class Seller
    {
        private Buyer curr;
        private Stand stand;
        public void Start(Stand stand)
        {
            this.stand = stand;
            Task.Run(() => Work());
        }
        //Work with buyer
        private void  Work()
        {
            while(stand.IsOpen)
            {

                
                if (!stand.Buyers.TryDequeue(out curr))
                {
                    //if queue to stand is empty seller want to have a rest 
                    Thread.Sleep(100);
                    continue;
                }

                Thread.Sleep(Helper.rnd.Next(10, 50));


                //lost some buyers:(
                if (curr == null) { System.Console.WriteLine("rip"); return; }
                



                int buy = curr.Buy(stand.StandProduct);
                Interlocked.Add(ref stand.Selled, buy);
                Interlocked.Increment(ref stand.RIPBuyers);
                curr.CallToQueue();
                
            }
        }
    }
}

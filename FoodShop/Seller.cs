using System.Threading;
using System.Threading.Tasks;

namespace FoodShop
{
    delegate int DelegateForSeller(Stand stand);
    class Seller
    {
        private Buyer curr;
        private Stand stand;
        private int standid;

        public event DelegateForSeller StandId;

        public void Start(Stand stand)
        {
            this.stand = stand;
            standid = StandId(stand);
            Task.Run(() => Work());
        }

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
                //Thread.Sleep(stand.rnd.Next(10, 50));
                curr.Stands[standid] = true;

                int buy = stand.rnd.Next(1, 4);
                Interlocked.Add(ref stand.Selled, buy);
                Interlocked.Increment(ref stand.RIPBuyers);

                curr.CallToQueue();
            }
        }
    }
}

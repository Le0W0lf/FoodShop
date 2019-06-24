using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodShop
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Product.Cake.ToString());
           /* Shop shop = new Shop();
            shop.Open();
            Buyer buyer = new Buyer();
            shop.Add(buyer);
            Thread.Sleep(1000);
            shop.Statistic();
            Thread.Sleep(1000);
            shop.Statistic();
            Console.Read();*/
            
            User user = new User();

            Console.WriteLine("\"open\"      for open shop");
            Console.WriteLine("\"close\"     for close shop");
            Console.WriteLine("\"stats\"     for see statistics");
            Console.WriteLine("\"start X Y\" for start sending X buyers per each Y seconds to the shop");
            Console.WriteLine("\"stop\"      for stop sending buyers");
            Console.WriteLine("\"exit\"      for close program");
            while (true)
            {
                var s = (Console.ReadLine()).ToLower().Split(' ');
                switch(s[0])
                {
                    case "open":
                        user.Open();
                        break;
                    case "close":
                        user.Close();
                        break;
                    case "stats":
                        user.Statistic();
                        break;
                    case "start":
                        int x=0;
                        double y=0;
                        if (s.Length != 3 || !int.TryParse(s[1], out x) || !double.TryParse(s[2], out y)) 
                        {
                            Console.WriteLine("Somethings go wrong. :( Try again");
                            break;
                        }
                        user.Start(x,y);
                        break;
                    case "stop":
                        user.Stop();
                        break;
                    case "exit":
                        user.Close();
                        return;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }

            }
            //Console.Read();
            
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop
{
    [Flags]
    public enum Product
    {
        IceCream = 1,
        Cake = 2,
        Chocolate = 4
    };
}

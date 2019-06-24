using FoodShop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoodShopTest
{
    [TestClass]
    public class StandTest
    {
        Stand sut;

        [TestInitialize]
        public void Init()
        {
            sut = new Stand("Cake", 10, Product.Cake);

        }

        [TestMethod]
        public void AddBuyers()
        {
        }
    }
}

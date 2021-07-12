using System;
using NUnit.Framework;

namespace Financier.Common.Tests.Expenses.Models.Item
{
    public class GetAllMonthsTests : InitializedDatabaseTests
    {
        [Test]
        public void Test_Expenses_Models_Item_GetAllMonths()
        {
            var actual = Financier.Common.Expenses.Models.Item.GetAllMonths();

            var expected = new DateTime[]
            {
                new DateTime(2019, 6, 1),
                new DateTime(2019, 7, 1),
                new DateTime(2019, 8, 1),
                new DateTime(2019, 9, 1),
            };

            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}

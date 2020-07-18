using System;
using NUnit.Framework;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.Models.ItemTests
{
    public class GetAllMonths : DatabaseFixture
    {
        [Test]
        public void Test_Expenses_Models_Item_GetAllMonths()
        {
            var actual = Item.GetAllMonths();

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

using System;
using NUnit.Framework;

using Financier.Common.Liabilities;

namespace Financier.Common.Tests.Liabilities.MortgagesTests
{
    public class GetFixedRateMortgageTest
    {
        public DateTime PurchasedAt { get; private set; }
        public decimal InterestRate { get; private set; }
        public decimal MortgageValue { get; private set; }
        public int AmortisationPeriodInMonths { get; private set; }

        [SetUp]
        public void Init()
        {
            PurchasedAt = new DateTime(2019, 1, 1);
            InterestRate = 0.0319M;
            MortgageValue = 328000.00M;
            AmortisationPeriodInMonths = 300;
        }

        [TestCase(40000.00)]
        [TestCase(80000.00)]
        [TestCase(81000.00)]
        [TestCase(81999.99)]
        public void Test_GetsInsuredMortgage(decimal downPayment)
        {
            var actual = Mortgages.GetFixedRateMortgage(
                MortgageValue,
                InterestRate,
                AmortisationPeriodInMonths,
                PurchasedAt,
                downPayment
            );

            Assert.That(actual, Is.TypeOf<InsuredMortgage>());
        }

        [TestCase(82000)]
        [TestCase(83000.00)]
        [TestCase(400000.00)]
        [TestCase(82000.00)]
        public void Test_GetsFixedRateMortgage(decimal downPayment)
        {
            var actual = Mortgages.GetFixedRateMortgage(
                MortgageValue,
                InterestRate,
                AmortisationPeriodInMonths,
                PurchasedAt,
                downPayment
            );

            Assert.That(actual, Is.TypeOf<FixedRateMortgage>());
        }
    }
}

using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities.MortgagesTests
{
    public class GetFixedRateMortgageTest
    {
        public DateTime PurchasedAt { get; private set; }
        public decimal InterestRate { get; private set; }
        public Money MortgageValue { get; private set; }
        public int AmortisationPeriodInMonths { get; private set; }

        [SetUp]
        public void Init()
        {
            PurchasedAt = new DateTime(2019, 1, 1);
            InterestRate = 0.0319M;
            MortgageValue = new Money(328000.00M, PurchasedAt);
            AmortisationPeriodInMonths = 300;
        }

        [TestCase(0.20, 0.00)]
        [TestCase(0.20, 40000.00)]
        [TestCase(0.20, 80000.00)]
        [TestCase(0.20, 81000.00)]
        [TestCase(0.20, 81999.99)]
        public void Test_GetsInsuredMortgage(decimal maximumInsuranceRate, decimal downPayment)
        {
            var actual = Mortgages.GetFixedRateMortgage(
                MortgageValue,
                InterestRate,
                AmortisationPeriodInMonths,
                PurchasedAt,
                new Money(downPayment, PurchasedAt),
                maximumInsuranceRate
            );

            Assert.That(actual, Is.TypeOf<InsuredMortgage>());
        }

        [TestCase(0.20, 82000)]
        [TestCase(0.20, 83000.00)]
        [TestCase(0.20, 400000.00)]
        [TestCase(0.10, 41000.00)]
        public void Test_GetsFixedRateMortgage(decimal maximumInsuranceRate, decimal downPayment)
        {
            var actual = Mortgages.GetFixedRateMortgage(
                    MortgageValue,
                    InterestRate,
                    AmortisationPeriodInMonths,
                    PurchasedAt,
                    new Money(downPayment, PurchasedAt),
                    maximumInsuranceRate
                    );

            Assert.That(actual, Is.TypeOf<FixedRateMortgage>());
        }
    }
}

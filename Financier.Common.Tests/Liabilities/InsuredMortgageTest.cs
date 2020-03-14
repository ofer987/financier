using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities
{
    public class InsuredMortgageTest
    {
        [TestCase(0.05, 19000.00)]
        [TestCase(0.10, 13950.00)]
        [TestCase(0.11, 13795.00)]
        [TestCase(0.12, 13640.00)]
        [TestCase(0.13, 13485.00)]
        [TestCase(0.14, 13330.00)]
        [TestCase(0.15, 11900.00)]
        [TestCase(0.16, 11760.00)]
        [TestCase(0.17, 11620.00)]
        [TestCase(0.18, 11480.00)]
        [TestCase(0.19, 11340.00)]
        [TestCase(0.199, 11340.00)]
        public void Test_InsuranceValue(decimal downPaymentPercentage, decimal expected)
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var homePrice = 500000.00M;
            var downPayment = new Money(
                downPaymentPercentage * homePrice,
                purchasedAt
            );
            var mortgageAmount = new Money(
                (1 - downPaymentPercentage) * homePrice,
                purchasedAt
            );
            var preferredInterestRate = 0.0319M;

            var baseMortgage = new FixedRateMortgage(mortgageAmount, preferredInterestRate, 300, purchasedAt);
            var subject = new InsuredMortgage(baseMortgage, downPayment);

            Assert.That(
                subject.Insurance.Value,
                Is.EqualTo(expected).Within(15.00M).Percent
            );
        }
    }
}

using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities
{
    public class PrepayableMortgageTest
    {
        public DateTime PurchasedAt => Subject.InitiatedAt;
        public Financier.Common.Liabilities.FixedRateMortgage Mortgage { get; private set; }
        public Financier.Common.Liabilities.PrepayableMortgage Subject { get; private set; }

        [SetUp]
        public void Init()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;
            Mortgage = new FixedRateMortgage(mortgageAmountMoney, preferredInterestRate, 300, purchasedAt);
            Subject = new Financier.Common.Liabilities.PrepayableMortgage(Mortgage, 0.10M);
            Subject.AddPrepayment(new DateTime(2019, 12, 31), 32800.00M);
            Subject.AddPrepayment(new DateTime(2020, 12, 31), 32800.00M);
        }

        [Test]
        public void Test_AddPrepayment_CannotAddOver10Percent()
        {
            Assert.Throws<OverPaymentException>(() => 
            {
                Subject.AddPrepayment(new DateTime(2019, 12, 30), 1.00M);
            });
        }

        [Test]
        public void Test_MonthlyPayment()
        {
            Assert.That(
                decimal.Round(
                    Convert.ToDecimal(Subject.MonthlyPayment),
                    2),
                Is.EqualTo(1584.40)
            );
        }

        [TestCase(2019, 1, 1, 328000)]
        [TestCase(2019, 1, 15, 327287.54)]
        [TestCase(2019, 1, 31, 327287.54)]
        [TestCase(2019, 2, 1, 327287.54)]
        [TestCase(2019, 2, 2, 326573.18)]
        [TestCase(2019, 12, 31, 319324.30)]
        [TestCase(2020, 1, 1, 286524.31)]
        [TestCase(2020, 1, 2, 285701.59)]
        [TestCase(2037, 7, 1, 65.18)]
        [TestCase(2037, 7, 2, 0)]
        [TestCase(2037, 8, 1, 0)]
        [TestCase(2037, 9, 1, 0)]
        [TestCase(2037, 10, 1, 0)]
        [TestCase(2037, 11, 1, 0)]
        [TestCase(2044, 1, 1, 0)]
        public void Test_GetBalance(int year, int month, int day, decimal expected)
        {
            Assert.That(
                Subject.GetBalance(new DateTime(year, month, day)).Value,
                Is.EqualTo(expected)
            );
        }

        [TestCase(1, 871.93)]
        [TestCase(2, 870.04)]
        [TestCase(5, 864.33)]
        [TestCase(10, 854.71)]
        [TestCase(12, 850.82)]
        [TestCase(13, 0)]
        [TestCase(14, 761.68)]
        [TestCase(25, 737.30)]
        [TestCase(26, 0)]
        [TestCase(27, 647.85)]
        [TestCase(200, 101.91)]
        [TestCase(224, 4.37)]
        [TestCase(225, 0.17)]
        public void Test_GetMonthlyInterestPayment(int monthCount, decimal expectedInterestPayment)
        {
            Assert.That(
                Subject.GetMonthlyPayments()
                    .Select(payment => payment.Interest.Value)
                    .ToList()[monthCount]
                , Is.EqualTo(expectedInterestPayment)
            );
        }

        [TestCase(1, 712.46)]
        [TestCase(2, 714.36)]
        [TestCase(5, 720.07)]
        [TestCase(10, 729.69)]
        [TestCase(12, 733.58)]
        [TestCase(13, 32800)]
        [TestCase(14, 822.72)]
        [TestCase(25, 847.10)]
        [TestCase(26, 32800)]
        [TestCase(27, 936.55)]
        [TestCase(200, 1482.49)]
        [TestCase(224, 1580.02)]
        [TestCase(225, 65.17)]
        public void Test_GetMonthlyPrincipalPayment(int index, decimal expectedPrincipalPayment)
        {
            Assert.That(
                Subject.GetMonthlyPayments()
                    .Select(payment => payment.Principal.Value)
                    .ToList()[index]
                , Is.EqualTo(expectedPrincipalPayment)
            );
        }
    }
}

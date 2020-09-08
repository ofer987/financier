using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Liabilities;

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
            var preferredInterestRate = 0.0319M;
            Mortgage = new FixedRateMortgage(mortgageAmount, preferredInterestRate, 300, purchasedAt);
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

        [TestCase(2019, 1, 1, 328000.00)]
        [TestCase(2019, 1, 2, 327287.54)]
        [TestCase(2019, 1, 15, 327287.54)]
        [TestCase(2019, 1, 31, 327287.54)]
        [TestCase(2019, 2, 1, 327287.54)]
        [TestCase(2019, 2, 2, 326573.18)]
        [TestCase(2019, 12, 31, 319324.30)]
        [TestCase(2020, 1, 1, 286524.31)]
        [TestCase(2020, 1, 2, 285701.59)]
        [TestCase(2021, 1, 1, 243706.02)]
        [TestCase(2021, 12, 31, 232301.69)]
        [TestCase(2022, 1, 1, 199501.69)]
        [TestCase(2023, 1, 1, 153866.43)]
        [TestCase(2024, 1, 1, 106753.94)]
        [TestCase(2025, 1, 1, 58116.39)]
        [TestCase(2026, 1, 1, 7904.41)]
        [TestCase(2026, 6, 1, 45.81)]
        [TestCase(2026, 6, 2, 0)]
        [TestCase(2037, 7, 1, 0)]
        [TestCase(2037, 7, 2, 0)]
        [TestCase(2037, 8, 1, 0)]
        [TestCase(2037, 9, 1, 0)]
        [TestCase(2037, 10, 1, 0)]
        [TestCase(2037, 11, 1, 0)]
        [TestCase(2044, 1, 1, 0)]
        public void Test_PrepayableMortgage_GetBalance(int year, int month, int day, decimal expected)
        {
            Subject.AddPrepayment(new DateTime(2021, 12, 31), 32800.00M);
            Subject.AddPrepayment(new DateTime(2022, 12, 31), 32800.00M);
            Subject.AddPrepayment(new DateTime(2023, 12, 31), 32800.00M);
            Subject.AddPrepayment(new DateTime(2024, 12, 31), 32800.00M);
            Subject.AddPrepayment(new DateTime(2025, 12, 31), 32800.00M);
            Subject.AddPrepayment(new DateTime(2026, 12, 31), 32800.00M);
            Assert.That(
                Subject.GetBalance(new DateTime(year, month, day)),
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
                    .Select(payment => payment.Interest)
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
                    .Select(payment => payment.Principal)
                    .ToList()[index]
                , Is.EqualTo(expectedPrincipalPayment)
            );
        }

        [TestCase(2020, 1, 31, 2020, 1, 28)]
        [TestCase(2020, 1, 28, 2020, 1, 28)]
        [TestCase(2020, 2, 28, 2020, 2, 28)]
        [TestCase(2019, 1, 29, 2019, 1, 28)]
        [TestCase(2020, 12, 31, 2020, 12, 28)]
        [TestCase(2010, 12, 31, 2010, 12, 28)]
        public void Test_Constructor(int year, int month, int day, int expectedYear, int expectedMonth, int expectedDay)
        {
            var date = new DateTime(year, month, day);
            var money = 100.00M;

            var baseMortgage = new FixedRateMortgage(money, 0.0314M, 300, date);
            var mortgage = new PrepayableMortgage(baseMortgage);

            Assert.That(mortgage.InitiatedAt.Year, Is.EqualTo(expectedYear));
            Assert.That(mortgage.InitiatedAt.Month, Is.EqualTo(expectedMonth));
            Assert.That(mortgage.InitiatedAt.Day, Is.EqualTo(expectedDay));
        }
    }
}

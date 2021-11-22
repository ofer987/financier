using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

using Financier.Common.Models;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class SaleTest
    {
        [AllowNull]
        public Product Television { get; private set; }

        [AllowNull]
        public Product Stand { get; private set; }

        [AllowNull]
        public Product House { get; private set; }

        public static DateTime InitiatedAt = new DateTime(2020, 1, 1);

        [SetUp]
        public void Init()
        {
            Television = new SimpleProduct("television", new Money(40.00M, InitiatedAt));
            Stand = new SimpleProduct("stand", new Money(20.00M, InitiatedAt));
            House = new SimpleProduct("stand", new Money(5000.00M, InitiatedAt));
        }

        public void Test_SetNext_CannotBeBefore()
        {
            var at = InitiatedAt;
            var salePrice = new Money(100.00M, at);

            var purchase = new Purchase(Television, at);
            var sale = new Sale(Television, salePrice, at);
            purchase.Next = sale;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                sale.Next = new Purchase(Television, at.AddDays(-1));
            });
        }

        public void Test_SetNext_CanBeAtSameTime()
        {
            var at = InitiatedAt;
            var salePrice = new Money(100.00M, at);

            var purchase = new Purchase(Television, at);
            var sale = new Sale(Television, salePrice, at);
            purchase.Next = sale;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                sale.Next = new Purchase(Television, at);
            });
        }

        public void Test_SetNext_CanBeAfter()
        {
            var at = InitiatedAt;
            var salePrice = new Money(100.00M, at);

            var purchase = new Purchase(Television, at);
            var sale = new Sale(Television, salePrice, at);
            purchase.Next = sale;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                sale.Next = new Purchase(Television, at.AddDays(1));
            });
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;
using Financier.Common.Models.Income;

namespace Financier.Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var incomeStatement = CalculateIncomeStatement(GetPerson());

            Console.WriteLine(incomeStatement.ToString());
        }

        private static Person GetPerson()
        {
            Home home;
            {
                var purchasedAt = new DateTime(2018, 1, 1);
                var expenses = new Dictionary<string, decimal>
                {
                    { "taxes", 100.00M },
                    { "electricity", 50.00M }
                };
                home = new Home("Where-we-live", 200000, expenses, 40000, purchasedAt, 3.19M);
            }
            var products = new [] { home };
            var income = new[] { new Salary(200000.00M) };
            var person = new Person(products, income);

            return person;
        }

        private static IncomeStatement CalculateIncomeStatement(Person person)
        {
            return new IncomeStatement(0, person.IncomeSources, person.Products, new DateTime(2018, 1, 1), new DateTime(2020, 1, 1));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

using Financier.Common.Expenses.Models;
using Financier.Common.Models;
using Financier.Common.Expenses;
using Financier.Common.Liabilities;
using Financier.Web.GraphQL.CashFlows;
using Financier.Web.GraphQL.Liabilities;

namespace Financier.Web.GraphQL.Expenses
{
    public class MyHomeQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string InitialCash = "initialCash";
            public static string InitialDebt = "initialDebt";
            public static string PurchasedAt = "purchasedAt";
            public static string Name = "name";
            public static string DownPayment = "downPayment";
            public static string MortgageAmount = "mortgageAmount";
            public static string InterestRate = "interestRate";
            public static string AmortisationPeriodInMonths = "amortisationPeriodInMonths";
            public static string InflationType = "inflationType";
        }

        public MyHomeQuery()
        {
            Field<ListGraphType<MonthlyPaymentType>>(
                "getFixedMortgagePaymentsWithPrepayments",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InitialCash
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InitialDebt
                    },
                    new QueryArgument<NonNullGraphType<DateGraphType>>
                    {
                        Name = Keys.PurchasedAt
                    },
                    new QueryArgument<StringGraphType>
                    {
                        Name = Keys.Name,
                        DefaultValue = "Loud Train Lady"
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.DownPayment
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.MortgageAmount
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InterestRate
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = Keys.AmortisationPeriodInMonths,
                        DefaultValue = 300
                    },
                    new QueryArgument<InflationType>
                    {
                        Name = Keys.InflationType,
                        DefaultValue = InflationTypes.NoopInflation
                    }
                ),
                resolve: context =>
                {
                    var homeName = context.GetArgument<string>(Keys.Name);
                    var purchasedAt = context.GetArgument<DateTime>(Keys.PurchasedAt);
                    var downPayment = context.GetArgument<decimal>(Keys.DownPayment);
                    var home = new Home(homeName, purchasedAt, downPayment);

                    var baseValue = context.GetArgument<decimal>(Keys.MortgageAmount);
                    var baseValueMoney = new Money(baseValue, purchasedAt);
                    var interestRate = context.GetArgument<decimal>(Keys.InterestRate);
                    var amortisationPeriodInMonths = context.GetArgument<int>(Keys.AmortisationPeriodInMonths);
                    var mortgage = new FixedRateMortgage(home, baseValueMoney, interestRate, amortisationPeriodInMonths);

                    var cashFlow = CreateCashFlow();

                    var initialCash = context.GetArgument<decimal>(Keys.InitialCash);
                    var initialDebt = context.GetArgument<decimal>(Keys.InitialDebt);
                    var financialStatement = MyHome.BuildStatementWithPrepaybleMortgage(
                        mortgage,
                        cashFlow,
                        initialCash,
                        initialDebt
                    );

                    var inflationType = context.GetArgument<InflationTypes>(Keys.InflationType);
                    var inflation = Inflations.GetInflation(inflationType);

                    return financialStatement.Mortgage.GetMonthlyPayments()
                        .Select(payment => payment.GetValueAt(inflation, purchasedAt))
                        .ToList();
                }
            );

            Field<ListGraphType<MonthlyPaymentType>>(
                "getFixedMortgagePayments",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InitialCash
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InitialDebt
                    },
                    new QueryArgument<NonNullGraphType<DateGraphType>>
                    {
                        Name = Keys.PurchasedAt
                    },
                    new QueryArgument<StringGraphType>
                    {
                        Name = Keys.Name,
                        DefaultValue = "Loud Train Lady"
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.DownPayment
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.MortgageAmount
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InterestRate
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = Keys.AmortisationPeriodInMonths,
                        DefaultValue = 300
                    },
                    new QueryArgument<InflationType>
                    {
                        Name = Keys.InflationType,
                        DefaultValue = InflationTypes.NoopInflation
                    }
                ),
                resolve: context =>
                {
                    var homeName = context.GetArgument<string>(Keys.Name);
                    var purchasedAt = context.GetArgument<DateTime>(Keys.PurchasedAt);
                    var downPayment = context.GetArgument<decimal>(Keys.DownPayment);
                    var home = new Home(homeName, purchasedAt, downPayment);

                    var baseValue = context.GetArgument<decimal>(Keys.MortgageAmount);
                    var baseValueMoney = new Money(baseValue, purchasedAt);
                    var interestRate = context.GetArgument<decimal>(Keys.InterestRate);
                    var amortisationPeriodInMonths = context.GetArgument<int>(Keys.AmortisationPeriodInMonths);
                    var mortgage = new FixedRateMortgage(home, baseValueMoney, interestRate, amortisationPeriodInMonths);

                    var cashFlow = CreateCashFlow();

                    var initialCash = context.GetArgument<decimal>(Keys.InitialCash);
                    var initialDebt = context.GetArgument<decimal>(Keys.InitialDebt);
                    var financialStatement = MyHome.BuildStatementWithMortgage(mortgage, cashFlow, initialCash, initialDebt);

                    var inflationType = context.GetArgument<InflationTypes>(Keys.InflationType);
                    var inflation = Inflations.GetInflation(inflationType);

                    return financialStatement.Mortgage.GetMonthlyPayments()
                        .Select(payment => payment.GetValueAt(inflation, purchasedAt))
                        .ToList();
                }
            );
        }

        private ICashFlow CreateCashFlow()
        {
            var startAt = Item.GetAll()
                .OrderBy(item => item.At)
                .Select(item => item.At)
                .First();

            var endAt = Item.GetAll()
                .OrderBy(item => item.At)
                .Select(item => item.At)
                .Last();

            return new CashFlow(startAt, endAt);
        }
    }
}

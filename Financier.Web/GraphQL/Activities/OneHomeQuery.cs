using System;
using GraphQL;
using GraphQL.Types;

using Financier.Common.Expenses.Actions;
using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Web.GraphQL.Activities
{
    // TODO: maybe rename to HomeQuery????
    public class OneHomeQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string InitiatedAt = "initiatedAt";
            public static string DownPayment = "downPayment";
            public static string PurchasePrice = "purchasePrice";
            public static string AmortisationPeriodInMonths = "amortisationPeriodInMonths";
            public static string InterestRate = "interestRate";
            public static string MonthlyRentalRate = "monthlyRentalRate";
        }

        public OneHomeQuery()
        {
            Field<DecimalGraphType>(
                "buyAndSellCondo",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<DateGraphType>>
                    {
                        Name = Keys.InitiatedAt
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.DownPayment
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.PurchasePrice
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = Keys.AmortisationPeriodInMonths,
                        DefaultValue = 300
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InterestRate
                    }
                ),
                resolve: context =>
                {
                    Console.WriteLine("Hello");
                    var purchasedAt = GetInitiatedAt(context);
                    var purchasePrice = GetPurchasePrice(context);
                    var downPayment = GetDownPayment(context);
                    var interestRate = GetInterestRate(context);
                    var amortisationPeriodInMonths = GetAmortizationPeriodInMonths(context);
                    var soldAt = purchasedAt.AddMonths(amortisationPeriodInMonths);
                    var salePrice = Inflations.CondoPriceIndex.GetValueAt(purchasePrice, purchasedAt, soldAt);

                    // START Validation
                    // END Validation

                    var mortgage = new FixedRateMortgage(purchasePrice - downPayment, interestRate, amortisationPeriodInMonths, purchasedAt);
                    var home = new Home("foobar", purchasedAt, purchasePrice, downPayment, mortgage);
                    var activity = new Activity(purchasedAt);


                    activity.Buy(home, purchasedAt);
                    activity.Sell(home, salePrice, soldAt);
                    // var year = context.GetArgument<int>(Keys.Year);
                    // var month = context.GetArgument<int>(Keys.Month);
                    // var day = context.GetArgument<int>(Keys.Day);
                    //
                    // var at = new DateTime(year, month, day);

                    return activity.GetNetWorthAt(Inflations.NoopInflation, soldAt.AddDays(1));
                }
            );
            Field<DecimalGraphType>(
                "rentApartment",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<DateGraphType>>
                    {
                        Name = Keys.InitiatedAt
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = Keys.AmortisationPeriodInMonths,
                        DefaultValue = 300,
                        Description = "Rental Period (in months)"
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.MonthlyRentalRate
                    }
                ),
                resolve: context =>
                {
                    var startAt = GetInitiatedAt(context);
                    var rentalPeriod = GetAmortizationPeriodInMonths(context);
                    var rentalMonthlyRate = GetMonthlyRentalRate(context);
                    var endAt = startAt.AddMonths(rentalPeriod);

                    var rental = new RentalHome("foobar", startAt, rentalMonthlyRate);

                    // START Validation
                    // END Validation

                    var activity = new Activity(startAt);


                    activity.Buy(rental, startAt);
                    activity.Sell(rental, 0.00M, endAt);

                    return activity.GetNetWorthAt(Inflations.NoopInflation, endAt.AddDays(1));
                }
            );
        }

        private DateTime GetInitiatedAt(IResolveFieldContext<object> context)
        {
            return context.GetArgument<DateTime>(Keys.InitiatedAt);
        }

        private decimal GetPurchasePrice(IResolveFieldContext<object> context)
        {
            return context.GetArgument<decimal>(Keys.PurchasePrice);
        }

        private decimal GetDownPayment(IResolveFieldContext<object> context)
        {
            return context.GetArgument<decimal>(Keys.DownPayment);
        }

        private int GetAmortizationPeriodInMonths(IResolveFieldContext<object> context)
        {
            return context.GetArgument<int>(Keys.AmortisationPeriodInMonths);
        }

        private decimal GetInterestRate(IResolveFieldContext<object> context)
        {
            return context.GetArgument<decimal>(Keys.InterestRate);
        }

        private decimal GetMonthlyRentalRate(IResolveFieldContext<object> context)
        {
            return context.GetArgument<decimal>(Keys.MonthlyRentalRate);
        }
    }
}

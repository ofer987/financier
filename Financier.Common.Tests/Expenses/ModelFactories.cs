using System;

namespace Financier.Common.Tests.Expenses
{
    public class FactoryData
    {
        public class Accounts
        {
            public class Dan
            {
                public const string AccountName = "Dan";

                public class Cards
                {
                    public class Savings
                    {
                        public const string CardNumber = "123143287999";

                        public class Statements
                        {
                            public class June
                            {
                                public static DateTime PostedAt = new DateTime(2019, 6, 30);
                                public class Items
                                {
                                    public class DanSalary
                                    {
                                        public const string ItemId = "gfhkj35hkls";
                                        public const decimal Amount = -2000.00M;
                                        public const string Description = "Dan Salary";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 6);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 5);
                                    }

                                    public class EdithSalary
                                    {
                                        public const string ItemId = "347js9";
                                        public const decimal Amount = -1000.00M;
                                        public const string Description = "Edith Salary";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 23);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 23);
                                    }

                                    public class Groceries
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 104.50M;
                                        public const string Description = "Fresco";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 5);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 5);
                                    }

                                    public class Coffee
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 4.20M;
                                        public const string Description = "IQ";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 15);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 15);
                                    }

                                    public class DanCreditCardPayment
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 967.15M;
                                        public const string Description = "Transfer to Dan Credit Card";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 22);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 22);
                                    }

                                    public class CrazyCreditCardPayment
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 35000.00M;
                                        public const string Description = "Transfer to Crazy Credit Card";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 22);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 22);
                                    }

                                    public class ChildCareBenefit
                                    {
                                        public const string ItemId = "898ghg";
                                        public const decimal Amount = -800.00M;
                                        public const string Description = "Federal Childcare Benefit";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 23);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 23);
                                    }
                                }
                            }

                            public class July
                            {
                                public static DateTime PostedAt = new DateTime(2019, 7, 31);
                                public class Items
                                {
                                    public class DanSalary
                                    {
                                        public const string ItemId = "fdjg65201j";
                                        public const decimal Amount = -2000.00M;
                                        public const string Description = "Dan Salary";
                                        public static DateTime PostedAt = new DateTime(2019, 7, 6);
                                        public static DateTime TransactedAt = new DateTime(2019, 7, 5);
                                    }

                                    public class Groceries
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 98.25M;
                                        public const string Description = "Your Community Grocer";
                                        public static DateTime PostedAt = new DateTime(2019, 7, 8);
                                        public static DateTime TransactedAt = new DateTime(2019, 7, 7);
                                    }

                                    public class Coffee
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 4.20M;
                                        public const string Description = "IQ";
                                        public static DateTime PostedAt = new DateTime(2019, 7, 14);
                                        public static DateTime TransactedAt = new DateTime(2019, 7, 14);
                                    }

                                    public class DanCreditCardPayment
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 600000.00M;
                                        public const string Description = "Transfer to Dan Credit Card";
                                        public static DateTime PostedAt = new DateTime(2019, 7, 22);
                                        public static DateTime TransactedAt = new DateTime(2019, 7, 22);
                                    }

                                    public class ChildCareBenefit
                                    {
                                        public const string ItemId = "34h52";
                                        public const decimal Amount = -800.00M;
                                        public const string Description = "Federal Childcare Benefit";
                                        public static DateTime PostedAt = new DateTime(2019, 7, 23);
                                        public static DateTime TransactedAt = new DateTime(2019, 7, 23);
                                    }
                                }
                            }

                            public class August
                            {
                                public static DateTime PostedAt = new DateTime(2019, 8, 31);
                                public class Items
                                {
                                    public class Transfer
                                    {
                                        public const string ItemId = "fdjg65201j";
                                        public const decimal Amount = -20.00M;
                                        public const string Description = "An internal transfer";
                                        public static DateTime PostedAt = new DateTime(2019, 8, 16);
                                        public static DateTime TransactedAt = new DateTime(2019, 8, 15);
                                    }
                                }
                            }

                            public class September
                            {
                                public static DateTime PostedAt = new DateTime(2019, 9, 30);
                                public class Items
                                {
                                    public class DanSalary
                                    {
                                        public const string ItemId = "ghfjdkg8341";
                                        public const decimal Amount = -1000000.00M;
                                        public const string Description = "Dan Mega Salary";
                                        public static DateTime PostedAt = new DateTime(2019, 9, 21);
                                        public static DateTime TransactedAt = new DateTime(2019, 9, 21);
                                    }
                                }
                            }
                        }
                    }

                    public class DanCard
                    {
                        public const string CardNumber = "1234567";

                        public class Statements
                        {
                            public class June
                            {
                                public static DateTime PostedAt = new DateTime(2019, 7, 1);
                                public class Items
                                {
                                    public class Porsche
                                    {
                                        public const string ItemId = "1234";
                                        public const decimal Amount = 300000.00M;
                                        public const string Description = "Porsche 911";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 5);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 5);
                                    }

                                    public class Ferrari
                                    {
                                        public const string ItemId = "458487";
                                        public const decimal Amount = 300000.00M;
                                        public const string Description = "Ferrari";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 5);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 5);
                                    }

                                    public class CreditCardPayment
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = -967.15M;
                                        public const string Description = "Thank you!";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 22);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 22);
                                    }
                                }
                            }

                            public class July
                            {
                                public static DateTime PostedAt = new DateTime(2019, 8, 1);
                                public class Items
                                {
                                    public class Lunch
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = 10.00M;
                                        public const string Description = "Golden Star";
                                        public static DateTime PostedAt = new DateTime(2019, 7, 17);
                                        public static DateTime TransactedAt = new DateTime(2019, 7, 17);
                                    }

                                    public class CreditCardPayment
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = -600000.00M;
                                        public const string Description = "Thank you!";
                                        public static DateTime PostedAt = new DateTime(2019, 7, 22);
                                        public static DateTime TransactedAt = new DateTime(2019, 7, 22);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public class Ron
            {
                public const string AccountName = "Ron";

                public class Cards
                {
                    public class RonCard
                    {
                        public const string CardNumber = "34875487543";

                        public class Statements
                        {
                            public class Crazy
                            {
                                public static DateTime PostedAt = new DateTime(2019, 7, 1);

                                public class Items
                                {
                                    public class Lambo
                                    {
                                        public const string ItemId = "9481";
                                        public const decimal Amount = 300000.00M;
                                        public const string Description = "Lambo";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 5);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 5);
                                    }

                                    public class CreditCardPayment
                                    {
                                        public static string ItemId = Guid.NewGuid().ToString();
                                        public const decimal Amount = -35000.00M;
                                        public const string Description = "Thank you!";
                                        public static DateTime PostedAt = new DateTime(2019, 6, 22);
                                        public static DateTime TransactedAt = new DateTime(2019, 6, 22);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public class Tags
        {
            public class Fun
            {
                public const string Name = "Fun";
            }

            public class Fast
            {
                public const string Name = "Fast";
            }

            public class Dog
            {
                public const string Name = "Dog";
            }

            public class Groceries
            {
                public const string Name = "Groceries";
            }

            public class Coffee
            {
                public const string Name = "Coffee";
            }

            public class Lunch
            {
                public const string Name = "lunch";
            }

            public class CreditCardPayment
            {
                public const string Name = "credit-card-payment";
            }

            public class Salary
            {
                public const string Name = "salary";
            }

            public class Internal
            {
                public const string Name = "internal";
            }

            public class Savings
            {
                public const string Name = "savings";
            }
        }
    }
}

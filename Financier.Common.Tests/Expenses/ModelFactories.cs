using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses
{
    public class ModelFactories : Factories
    {
        public static class SavingsCard
        {
            public static Guid CardId = Guid.NewGuid();
            public const string CardNumber = "123143287999";
            public static Func<Account, Card> GetCard = (account) => new Card
            {
                Owner = account,
                Id = CardId,
                Number = CardNumber,
                CardType = CardTypes.Bank,
                Statements = new List<Statement>()
            };

            public static class June
            {
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = Guid.NewGuid(),
                    CardId = CardId,
                    PostedAt = new DateTime(2019, 6, 30)
                };

                public static class Items
                {
                    public const string DanSalaryItemId = "gfhkj35hkls";
                    public static Func<IEnumerable<Tag>, Item> GetDanSalary = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -2000.00M,
                        Description = "Dan Salary",
                        ItemId = DanSalaryItemId,
                        PostedAt = new DateTime(2019, 6, 6),
                        TransactedAt = new DateTime(2019, 6, 5),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };

                    public const string EdithSalaryItemId = "347js9";
                    public static Func<IEnumerable<Tag>, Item> GetEdithSalary = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -1000.00M,
                        Description = "Edith Salary",
                        ItemId = EdithSalaryItemId,
                        PostedAt = new DateTime(2019, 6, 23),
                        TransactedAt = new DateTime(2019, 6, 23),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };

                    public static Func<IEnumerable<Tag>, Item> GetGroceries = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 104.50M,
                        Description = "Fresco",
                        ItemId = Guid.NewGuid().ToString(),
                        PostedAt = new DateTime(2019, 6, 5),
                        TransactedAt = new DateTime(2019, 6, 5),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCoffee = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 4.20M,
                        Description = "IQ",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 15),
                        TransactedAt = new DateTime(2019, 6, 15)
                    };

                    public static Func<IEnumerable<Tag>, Item> GetDanCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 967.15M,
                        Description = "Transfer to Dan Credit Card",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 22),
                        TransactedAt = new DateTime(2019, 6, 22)
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCrazyCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 35000.00M,
                        Description = "Transfer to Crazy Credit Card",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 22),
                        TransactedAt = new DateTime(2019, 6, 22)
                    };

                    public const string ChildCareBenefitItemId = "898ghg";
                    public static Func<IEnumerable<Tag>, Item> GetChildCareBenefit = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -800.00M,
                        Description = "Federal Childcare Benefit",
                        ItemId = ChildCareBenefitItemId,
                        PostedAt = new DateTime(2019, 6, 23),
                        TransactedAt = new DateTime(2019, 6, 23),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };
                }
            }

            public static class July
            {
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = Guid.NewGuid(),
                    CardId = CardId,
                    PostedAt = new DateTime(2019, 7, 31)
                };

                public static class Items
                {
                    public const string DanSalaryItemId = "fdjg65201j";
                    public static Func<IEnumerable<Tag>, Item> GetDanSalary = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -2000.00M,
                        Description = "Dan Salary",
                        ItemId = DanSalaryItemId,
                        PostedAt = new DateTime(2019, 7, 6),
                        TransactedAt = new DateTime(2019, 7, 5),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };

                    public static Func<IEnumerable<Tag>, Item> GetGroceries = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 98.25M,
                        Description = "Your Community Grocer",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 7, 8),
                        TransactedAt = new DateTime(2019, 7, 7)
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCoffee = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 4.20M,
                        Description = "IQ",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 7, 14),
                        TransactedAt = new DateTime(2019, 7, 14)
                    };

                    public static Func<IEnumerable<Tag>, Item> GetDanCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 600000.00M,
                        Description = "Transfer to Dan Credit Card",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 7, 22),
                        TransactedAt = new DateTime(2019, 7, 22)
                    };

                    public const string ChildCareBenefitItemId = "34h52";
                    public static Func<IEnumerable<Tag>, Item> GetChildCareBenefit = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -800.00M,
                        Description = "Federal Childcare Benefit",
                        ItemId = ChildCareBenefitItemId,
                        PostedAt = new DateTime(2019, 7, 23),
                        TransactedAt = new DateTime(2019, 7, 23),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };
                }
            }

            public static class August
            {
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = Guid.NewGuid(),
                    CardId = CardId,
                    PostedAt = new DateTime(2019, 8, 31)
                };

                public static class Items
                {
                    public const string TransferItemId = "fdjg65201j";
                    public static Func<IEnumerable<Tag>, Item> GetTransfer = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -20.00M,
                        Description = "An internal transfer",
                        ItemId = TransferItemId,
                        PostedAt = new DateTime(2019, 8, 16),
                        TransactedAt = new DateTime(2019, 8, 15),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };
                }
            }

            public static class September
            {
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = Guid.NewGuid(),
                    CardId = CardId,
                    PostedAt = new DateTime(2019, 9, 30)
                };

                public static class Items
                {
                    public const string DanSalaryItemId = "ghfjdkg8341";
                    public static Func<IEnumerable<Tag>, Item> GetDanSalary = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -1000000.00M,
                        Description = "Dan Mega Salary",
                        ItemId = DanSalaryItemId,
                        PostedAt = new DateTime(2019, 9, 21),
                        TransactedAt = new DateTime(2019, 9, 21),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList()
                    };
                }
            }
        }

        public static class RonCard
        {
            public static Guid CardId = Guid.NewGuid();
            public const string CardNumber = "34875487543";
            public static Func<Account, Card> GetCard = (account) => new Card
            {
                Owner = account,
                Id = CardId,
                Number = CardNumber,
                CardType = CardTypes.Credit,
                Statements = new List<Statement>()
            };

            public static class CrazyStatement
            {
                public static Guid StatementId = Guid.NewGuid();
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = StatementId,
                    CardId = CardId,
                    Items = new List<Item>(),
                    PostedAt = new DateTime(2019, 7, 1)
                };

                public static class Items
                {
                    public const string LamboItemId = "9481";
                    public static Guid LamboId = Guid.NewGuid();
                    public static Func<IEnumerable<Tag>, Item> GetLamboItem = (tags) => new Item
                    {
                        Id = LamboId,
                        Amount = 300000.00M,
                        Description = "Lambo",
                        ItemId = LamboItemId,
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 5),
                        TransactedAt = new DateTime(2019, 6, 5),
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -35000.00M,
                        Description = "Thank you!",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 22),
                        TransactedAt = new DateTime(2019, 6, 22),
                    };
                }
            }
        }

        public static class DanCard
        {
            public static Guid CardId = Guid.NewGuid();
            public const string CardNumber = "1234567";
            public static Func<Account, Card> Card = (account) => new Card
            {
                Owner = account,
                Id = CardId,
                Number = CardNumber,
                CardType = CardTypes.Credit,
                Statements = new List<Statement>()
            };

            public static class June
            {
                public static Guid StatementId = Guid.NewGuid();
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = StatementId,
                    CardId = CardId,
                    Items = new List<Item>(),
                    PostedAt = new DateTime(2019, 7, 1)
                };

                public static class Items
                {
                    public const string PorscheItemId = "1234";
                    public static Guid PorscheId = Guid.NewGuid();
                    public static Func<IEnumerable<Tag>, Item> GetPorscheItem = (tags) => new Item
                    {
                        Id = PorscheId,
                        Amount = 300000.00M,
                        Description = "Porsche 911",
                        ItemId = PorscheItemId,
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 5),
                        TransactedAt = new DateTime(2019, 6, 5),
                    };

                    public const string FerrariItemId = "458487";
                    public static Guid FerrariId = Guid.NewGuid();
                    public static Func<IEnumerable<Tag>, Item> GetFerrariItem = (tags) => new Item
                    {
                        Id = FerrariId,
                        Amount = 300000.00M,
                        Description = "Ferrari",
                        ItemId = FerrariItemId,
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 5),
                        TransactedAt = new DateTime(2019, 6, 5),
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -967.15M,
                        Description = "Thank you!",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 22),
                        TransactedAt = new DateTime(2019, 6, 22),
                    };
                }
            }

            public static class July
            {
                public static Guid StatementId = Guid.NewGuid();
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = StatementId,
                    CardId = CardId,
                    Items = new List<Item>(),
                    PostedAt = new DateTime(2019, 8, 1)
                };

                public static class Items
                {
                    public static Func<IEnumerable<Tag>, Item> GetLunch = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 10.00M,
                        Description = "Golden Star",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 7, 17),
                        TransactedAt = new DateTime(2019, 7, 17),
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -600000.00M,
                        Description = "Thank you!",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 7, 22),
                        TransactedAt = new DateTime(2019, 7, 22),
                    };
                }
            }
        }

        public static class Accounts
        {
            public static Func<Account> GetMrBean = () => new Account
            {
                Name = "mr.bean@bbc.com"
            };
        }

        public static class Tags
        {
            public static Func<Tag> GetFun = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "Fun"
            };

            public static Func<Tag> GetFast = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "Fast"
            };

            public static Func<Tag> GetDog = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "like-a-dog"
            };

            public static Func<Tag> GetGroceries = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "groceries"
            };

            public static Func<Tag> GetCoffee = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "coffee"
            };

            public static Func<Tag> GetLunch = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "lunch"
            };

            public static Func<Tag> GetCreditCardPayment = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "credit-card-payment"
            };

            public static Func<Tag> GetSalary = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "salary"
            };

            public static Func<Tag> GetInternal = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "internal"
            };

            public static Func<Tag> GetSavings = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "savings"
            };
        }
    }
}

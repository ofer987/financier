using System;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.BankStatementImporterTests
{
    public class DoesItemExist : DatabaseAbstractFixture
    {
        public class MyFactories : Factories
        {
            public static Guid DanCardId = Guid.NewGuid();
            public const string DanCardNumber = "1234567";
            public static Func<Card> GetDanCard = () => new Card
            {
                Id = DanCardId,
                Number = DanCardNumber,
                Statements = new List<Statement>()
            };

            public static Guid JuneStatementId = Guid.NewGuid();
            public static Func<Statement> GetJuneStatement = () => new Statement
            {
                Id = JuneStatementId,
                CardId = DanCardId,
                Items = new List<Item>(),
                PostedAt = new DateTime(2025, 7, 1)
            };
            public static Guid Alminz_Id = Guid.NewGuid();
            public static string Alminz_ItemId = Guid.NewGuid().ToString();
            public static Item Alminz = new Item
            {
                Id = Alminz_Id,
                ItemId = Alminz_ItemId,
                StatementId = JuneStatementId,
                Description = "ALMINZ SNACKS",
                Amount = 14.00M,
                ItemTags = new List<ItemTag>(),
                PostedAt = new DateTime(2015, 7, 5),
                TransactedAt = new DateTime(2015, 7, 5)
            };
        }

        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    new BankStatementRecord
                    {
                        Number = MyFactories.DanCardNumber,
                        FirstBankCardNumber = "'5007660790617248'",
                        TransactionTypeString = "DEBIT",
                        PostedAt = MyFactories.Alminz.PostedAt.ToString("yyyyMMdd"),
                        Amount = "-14.00",
                        Description = MyFactories.Alminz.Description
                    }).Returns(true);

                yield return new TestCaseData(
                    new BankStatementRecord
                    {
                        Number = MyFactories.DanCardNumber,
                        FirstBankCardNumber = "'5007660790617248'",
                        TransactionTypeString = "DEBIT",
                        PostedAt = MyFactories.Alminz.PostedAt.ToString("yyyyMMdd"),
                        Amount = "-14.50",
                        Description = MyFactories.Alminz.Description
                    }).Returns(false);
            }
        }

        protected override void InitDb()
        {
            using (var db = new Context())
            {
                var danCard = MyFactories.GetDanCard();
                db.Cards.Add(danCard);
                db.SaveChanges();

                var juneStatement = MyFactories.GetJuneStatement();
                juneStatement.Items.Add(MyFactories.Alminz);

                db.Statements.Add(juneStatement);
                db.SaveChanges();
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public bool Test_Expenses_BankStatementImporter_DoesItemExist(BankStatementRecord record)
        {
            return new BankStatementImporter().DoesItemExist(record, MyFactories.JuneStatementId);
        }
    }
}

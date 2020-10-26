namespace Financier.Common.Tests.Expenses
{
    public abstract class DatabaseFixture : Fixture
    {
        protected override void InitStorage()
        {
            using (var db = new Context())
            {
                var funTag = Factories.CreateTag(FactoryData.Tags.Fun.Name);
                var fastTag = Factories.CreateTag(FactoryData.Tags.Fast.Name);
                var dogTag = Factories.CreateTag(FactoryData.Tags.Dog.Name);
                var groceriesTag = Factories.CreateTag(FactoryData.Tags.Groceries.Name);
                var coffeeTag = Factories.CreateTag(FactoryData.Tags.Coffee.Name);
                var lunchTag = Factories.CreateTag(FactoryData.Tags.Lunch.Name);
                var creditCardPaymentTag = Factories.CreateTag(FactoryData.Tags.CreditCardPayment.Name);
                var salaryTag = Factories.CreateTag(FactoryData.Tags.Salary.Name);
                var savingsTag = Factories.CreateTag(FactoryData.Tags.Savings.Name);
                var internalTag = Factories.CreateTag(FactoryData.Tags.Internal.Name);
                db.Tags.Add(funTag);
                db.Tags.Add(fastTag);
                db.Tags.Add(dogTag);
                db.Tags.Add(groceriesTag);
                db.Tags.Add(coffeeTag);
                db.Tags.Add(lunchTag);
                db.Tags.Add(creditCardPaymentTag);
                db.Tags.Add(salaryTag);
                db.SaveChanges();

                // Accounts
                var dan = Factories.CreateAccount(FactoryData.Accounts.Dan.AccountName);
                db.Accounts.Add(dan);

                var ron = Factories.CreateAccount(FactoryData.Accounts.Ron.AccountName);
                db.Accounts.Add(ron);

                // Credit Cards
                {
                    var danCard = Factories.CreateCreditCard(dan, FactoryData.Accounts.Dan.Cards.DanCard.CardNumber);
                    var ronCard = Factories.CreateCreditCard(ron, FactoryData.Accounts.Ron.Cards.RonCard.CardNumber);
                    db.Cards.Add(danCard);
                    db.Cards.Add(ronCard);
                    db.SaveChanges();

                    var juneStatement = Factories.CreateSimpleStatement(danCard, FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.PostedAt);
                    var julyStatement = Factories.CreateSimpleStatement(danCard, FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.PostedAt);
                    var ronsCrazyStatement = Factories.CreateSimpleStatement(ronCard, FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.PostedAt);
                    juneStatement.Items.AddRange(new[] {
                        Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId,
                            new[] { funTag, fastTag }
                        ),
                        Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId,
                            new[] { funTag }
                        ),
                        Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.CreditCardPayment.ItemId,
                            new[] { creditCardPaymentTag, internalTag }
                        )
                    });
                    julyStatement.Items.AddRange(new[] {
                        Factories.CreateItemWithTags(
                            julyStatement,
                            FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.CreditCardPayment.ItemId,
                            new[] { creditCardPaymentTag, internalTag }
                        ),
                        Factories.CreateItemWithTags(
                            julyStatement,
                            FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.Lunch.ItemId,
                            new[] { lunchTag }
                        )
                    });
                    ronsCrazyStatement.Items.AddRange(new[] {
                        Factories.CreateItemWithTags(
                            ronsCrazyStatement,
                            FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.ItemId,
                            new[] { fastTag, dogTag }
                        ),
                        Factories.CreateItemWithTags(
                            ronsCrazyStatement,
                            FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId,
                            new[] { creditCardPaymentTag, internalTag }
                        )
                    });

                    db.Statements.Add(juneStatement);
                    db.Statements.Add(julyStatement);
                    db.Statements.Add(ronsCrazyStatement);
                }

                // Bank Cards
                {
                    var savingsCard = Factories.CreateBankCard(dan, FactoryData.Accounts.Dan.Cards.Savings.CardNumber);
                    db.Cards.Add(savingsCard);
                    {
                        var juneStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.June.PostedAt);
                        juneStatement.Items.AddRange(new[] {
                            Factories.CreateItemWithTags(
                                juneStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.ItemId,
                                new[] { salaryTag }
                            ),
                            Factories.CreateItemWithTags(
                                juneStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.ItemId,
                                new[] { salaryTag }
                            ),
                            Factories.CreateItemWithTags(
                                juneStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Groceries.ItemId,
                                new[] { groceriesTag }
                            ),
                            Factories.CreateItemWithTags(
                                juneStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Coffee.ItemId,
                                new[] { groceriesTag }
                            ),
                            Factories.CreateItemWithTags(
                                juneStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanCreditCardPayment.ItemId,
                                new[] { creditCardPaymentTag, internalTag }
                            ),
                            Factories.CreateItemWithTags(
                                juneStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.CrazyCreditCardPayment.ItemId,
                                new[] { creditCardPaymentTag, internalTag }
                            ),
                            Factories.CreateItemWithTags(
                                juneStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.ItemId,
                                new[] { salaryTag }
                            ),
                        });
                        db.Statements.Add(juneStatement);

                        var julyStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.July.PostedAt);
                        julyStatement.Items.AddRange(new[] {
                            Factories.CreateItemWithTags(
                                julyStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.ItemId,
                                new[] { salaryTag }
                            ),
                            Factories.CreateItemWithTags(
                                julyStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Groceries.ItemId,
                                new[] { groceriesTag }
                            ),
                            Factories.CreateItemWithTags(
                                julyStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Coffee.ItemId,
                                new[] { coffeeTag }
                            ),
                            Factories.CreateItemWithTags(
                                julyStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanCreditCardPayment.ItemId,
                                new[] { creditCardPaymentTag, internalTag }
                            ),
                            Factories.CreateItemWithTags(
                                julyStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.ItemId,
                                new[] { salaryTag }
                            )
                        });
                        db.Statements.Add(julyStatement);

                        var augustStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.August.PostedAt);

                        augustStatement.Items.AddRange(new[] {
                            Factories.CreateItemWithTags(
                                augustStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.August.Items.Transfer.ItemId,
                                new[] { internalTag, savingsTag }
                            )
                        });
                        db.Statements.Add(augustStatement);

                        var septemberStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.September.PostedAt);
                        septemberStatement.Items.AddRange(new[] {
                            Factories.CreateItemWithTags(
                                septemberStatement,
                                FactoryData.Accounts.Dan.Cards.Savings.Statements.September.Items.DanSalary.ItemId,
                                new[] { salaryTag }
                            )
                        });
                        db.Statements.Add(septemberStatement);
                    }
                }

                db.SaveChanges();
            }
        }
    }
}

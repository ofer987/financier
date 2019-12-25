namespace Financier.Common.Tests.Expenses
{
    public abstract class DatabaseFixture : Fixture
    {
        protected override void InitStorage()
        {
            using (var db = new Context())
            {
                var funTag = ModelFactories.Tags.GetFun();
                var fastTag = ModelFactories.Tags.GetFast();
                var dogTag = ModelFactories.Tags.GetDog();
                var groceriesTag = ModelFactories.Tags.GetGroceries();
                var coffeeTag = ModelFactories.Tags.GetCoffee();
                var lunchTag = ModelFactories.Tags.GetLunch();
                var creditCardPaymentTag = ModelFactories.Tags.GetCreditCardPayment();
                var salaryTag = ModelFactories.Tags.GetSalary();
                var savingsTag = ModelFactories.Tags.GetSavings();
                var internalTag = ModelFactories.Tags.GetInternal();
                db.Tags.Add(funTag);
                db.Tags.Add(fastTag);
                db.Tags.Add(dogTag);
                db.Tags.Add(groceriesTag);
                db.Tags.Add(coffeeTag);
                db.Tags.Add(lunchTag);
                db.Tags.Add(creditCardPaymentTag);
                db.Tags.Add(salaryTag);
                db.SaveChanges();

                // Credit Cards
                {
                    var danCard = ModelFactories.DanCard.Card();
                    var ronCard = ModelFactories.RonCard.GetCard();
                    db.Cards.Add(danCard);
                    db.Cards.Add(ronCard);
                    db.SaveChanges();

                    var juneStatement = ModelFactories.DanCard.June.GetStatement();
                    var julyStatement = ModelFactories.DanCard.July.GetStatement();
                    var ronsCrazyStatement = ModelFactories.RonCard.CrazyStatement.GetStatement();
                    juneStatement.Items.AddRange(new[] {
                        ModelFactories.DanCard.June.Items.GetPorscheItem(new[] { funTag, fastTag }),
                        ModelFactories.DanCard.June.Items.GetFerrariItem(new[] { funTag }),
                        ModelFactories.DanCard.June.Items.GetCreditCardPayment(new[] { creditCardPaymentTag, internalTag })
                    });
                    julyStatement.Items.AddRange(new[] {
                        ModelFactories.DanCard.July.Items.GetCreditCardPayment(new[] { creditCardPaymentTag, internalTag }),
                        ModelFactories.DanCard.July.Items.GetLunch(new[] { lunchTag })
                    });
                    ronsCrazyStatement.Items.AddRange(new[] {
                        ModelFactories.RonCard.CrazyStatement.Items.GetLamboItem(new[] { fastTag, dogTag }),
                        ModelFactories.RonCard.CrazyStatement.Items.GetCreditCardPayment(new[] { creditCardPaymentTag, internalTag })
                    });

                    db.Statements.Add(juneStatement);
                    db.Statements.Add(julyStatement);
                    db.Statements.Add(ronsCrazyStatement);
                }

                // Bank Cards
                {
                    var bankCard = ModelFactories.SavingsCard.GetCard();
                    db.Cards.Add(bankCard);
                    {
                        var juneStatement = ModelFactories.SavingsCard.June.GetStatement();
                        juneStatement.Items.AddRange(new[] {
                            ModelFactories.SavingsCard.June.Items.GetDanSalary(new[] { salaryTag }),
                            ModelFactories.SavingsCard.June.Items.GetEdithSalary(new[] { salaryTag }),
                            ModelFactories.SavingsCard.June.Items.GetGroceries(new[] { groceriesTag }),
                            ModelFactories.SavingsCard.June.Items.GetCoffee(new[] { groceriesTag }),
                            ModelFactories.SavingsCard.June.Items.GetDanCreditCardPayment(new[] { creditCardPaymentTag, internalTag }),
                            ModelFactories.SavingsCard.June.Items.GetCrazyCreditCardPayment(new[] { creditCardPaymentTag, internalTag }),
                            ModelFactories.SavingsCard.June.Items.GetChildCareBenefit(new[] { salaryTag })
                        });
                        db.Statements.Add(juneStatement);

                        var julyStatement = ModelFactories.SavingsCard.July.GetStatement();
                        julyStatement.Items.AddRange(new[] {
                                ModelFactories.SavingsCard.July.Items.GetDanSalary(new[] { salaryTag }),
                            ModelFactories.SavingsCard.July.Items.GetGroceries(new[] { groceriesTag }),
                            ModelFactories.SavingsCard.July.Items.GetCoffee(new[] { coffeeTag }),
                            ModelFactories.SavingsCard.July.Items.GetDanCreditCardPayment(new[] { creditCardPaymentTag, internalTag } ),
                            ModelFactories.SavingsCard.July.Items.GetChildCareBenefit(new[] { salaryTag })
                        });
                        db.Statements.Add(julyStatement);

                        var augustStatement = ModelFactories.SavingsCard.August.GetStatement();

                        augustStatement.Items.AddRange(new[] {
                            ModelFactories.SavingsCard.August.Items.GetTransfer(new[] { internalTag, savingsTag })
                        });
                        db.Statements.Add(augustStatement);

                        var septemberStatement = ModelFactories.SavingsCard.September.GetStatement();
                        septemberStatement.Items.AddRange(new[] {
                            ModelFactories.SavingsCard.September.Items.GetDanSalary(new[] { salaryTag } )
                        });
                        db.Statements.Add(septemberStatement);
                    }
                }

                db.SaveChanges();
            }
        }
    }
}

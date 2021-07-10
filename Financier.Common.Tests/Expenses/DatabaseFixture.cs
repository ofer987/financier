namespace Financier.Common.Tests.Expenses
{
    // New Style of writing tests
    // You can use the FactoryData.* directly in the TestCase attributes
    public abstract class DatabaseFixture : Fixture
    {
        protected override void InitStorage()
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

            // Accounts
            var dan = Factories.CreateAccount(FactoryData.Accounts.Dan.AccountName);

            var ron = Factories.CreateAccount(FactoryData.Accounts.Ron.AccountName);

            // Credit Cards
            {
                var danCard = Factories.CreateCreditCard(dan, FactoryData.Accounts.Dan.Cards.DanCard.CardNumber);
                var ronCard = Factories.CreateCreditCard(ron, FactoryData.Accounts.Ron.Cards.RonCard.CardNumber);

                var juneStatement = Factories.CreateSimpleStatement(danCard, FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.PostedAt);
                var julyStatement = Factories.CreateSimpleStatement(danCard, FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.PostedAt);
                var ronsCrazyStatement = Factories.CreateSimpleStatement(ronCard, FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.PostedAt);
                Factories.CreateItemWithTags(
                        juneStatement,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.Description,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.PostedAt,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.Amount,
                        new[] { funTag, fastTag }
                        );
                Factories.CreateItemWithTags(
                        juneStatement,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.Description,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.PostedAt,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.Amount,
                        new[] { funTag }
                        );
                Factories.CreateItemWithTags(
                        juneStatement,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.CreditCardPayment.ItemId,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.CreditCardPayment.Description,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.CreditCardPayment.PostedAt,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.CreditCardPayment.Amount,
                        new[] { creditCardPaymentTag, internalTag }
                        );
                Factories.CreateItemWithTags(
                        julyStatement,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.CreditCardPayment.ItemId,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.CreditCardPayment.Description,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.CreditCardPayment.PostedAt,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.CreditCardPayment.Amount,
                        new[] { creditCardPaymentTag, internalTag }
                        );
                Factories.CreateItemWithTags(
                        julyStatement,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.Lunch.ItemId,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.Lunch.Description,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.Lunch.PostedAt,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.Lunch.Amount,
                        new[] { lunchTag }
                        );
                Factories.CreateItemWithTags(
                        ronsCrazyStatement,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.ItemId,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.Description,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.PostedAt,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.Amount,
                        new[] { fastTag, dogTag }
                        );
                Factories.CreateItemWithTags(
                        ronsCrazyStatement,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.Description,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.PostedAt,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.Amount,
                        new[] { creditCardPaymentTag, internalTag }
                        );
            }

            // Bank Cards
            {
                var savingsCard = Factories.CreateBankCard(dan, FactoryData.Accounts.Dan.Cards.Savings.CardNumber);
                {
                    var juneStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.June.PostedAt);
                    Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.Amount,
                            new[] { salaryTag }
                            );
                    Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.Amount,
                            new[] { salaryTag }
                            );
                    Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Groceries.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Groceries.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Groceries.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Groceries.Amount,
                            new[] { groceriesTag }
                            );
                    Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Coffee.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Coffee.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Coffee.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.Coffee.Amount,
                            new[] { groceriesTag }
                            );
                    Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanCreditCardPayment.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanCreditCardPayment.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanCreditCardPayment.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanCreditCardPayment.Amount,
                            new[] { creditCardPaymentTag, internalTag }
                            );
                    Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.CrazyCreditCardPayment.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.CrazyCreditCardPayment.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.CrazyCreditCardPayment.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.CrazyCreditCardPayment.Amount,
                            new[] { creditCardPaymentTag, internalTag }
                            );
                    Factories.CreateItemWithTags(
                            juneStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.Amount,
                            new[] { salaryTag }
                            );

                    var julyStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.July.PostedAt);
                    Factories.CreateItemWithTags(
                            julyStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.Amount,
                            new[] { salaryTag }
                            );
                    Factories.CreateItemWithTags(
                            julyStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Groceries.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Groceries.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Groceries.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Groceries.Amount,
                            new[] { groceriesTag }
                            );
                    Factories.CreateItemWithTags(
                            julyStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Coffee.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Coffee.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Coffee.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.Coffee.Amount,
                            new[] { coffeeTag }
                            );
                    Factories.CreateItemWithTags(
                            julyStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanCreditCardPayment.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanCreditCardPayment.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanCreditCardPayment.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanCreditCardPayment.Amount,
                            new[] { creditCardPaymentTag, internalTag }
                            );
                    Factories.CreateItemWithTags(
                            julyStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.Amount,
                            new[] { salaryTag }
                            );

                    var augustStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.August.PostedAt);

                    Factories.CreateItemWithTags(
                            augustStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.August.Items.Transfer.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.August.Items.Transfer.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.August.Items.Transfer.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.August.Items.Transfer.Amount,
                            new[] { internalTag, savingsTag }
                            );

                    var septemberStatement = Factories.CreateSimpleStatement(savingsCard, FactoryData.Accounts.Dan.Cards.Savings.Statements.September.PostedAt);
                    Factories.CreateItemWithTags(
                            septemberStatement,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.September.Items.DanSalary.ItemId,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.September.Items.DanSalary.Description,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.September.Items.DanSalary.PostedAt,
                            FactoryData.Accounts.Dan.Cards.Savings.Statements.September.Items.DanSalary.Amount,
                            new[] { salaryTag }
                            );
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class Analysis
    {
        public DateTime StartAt { get; }
        public DateTime EndAt { get; }

        public Analysis(DateTime startAt, DateTime endAt)
        {
            StartAt = startAt;
            EndAt = endAt;
        }

        public class Result
        {
            public string TagName { get; set; }
            public string ItemDescription { get; set; }
            public decimal ItemAmount { get; set; }
            public DateTime ItemTransactedAt { get; set; }
        }

        public IEnumerable<ValueTuple<Tag, Item>> GetItemsByTag()
        {
            using (var db = new Context())
            {
                return (
                     from t in db.Tags
                     join it in db.ItemTags on t.Id equals it.TagId
                     join i in db.Items on it.ItemId equals i.Id
                     where true
                         && i.TransactedAt >= StartAt
                         && i.TransactedAt < EndAt
                     select ValueTuple.Create<Tag, Item>(t, i)
                    ).ToList();
            }
        }

        public IEnumerable<ValueTuple<Tag, Item>> GetExpensesByTag()
        {
            using (var db = new Context())
            {
                return (
                    from t in db.Tags
                    join it in db.ItemTags on t.Id equals it.TagId
                    join i in db.Items on it.ItemId equals i.Id
                    where true
                        && i.TransactedAt >= StartAt
                        && i.TransactedAt < EndAt
                        && i.Amount > 0
                        && t.Name != "credit-card-payment"
                    select ValueTuple.Create<Tag, Item>(t, i)
                   ).ToList();
            }
        }

        public decimal GetExpenses()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Where(item => item.Statement.Card.CardType == CardTypes.Bank)
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt)
                    .ToList()
                    .Aggregate(0.00M, (result, item) => result + item.Amount);
            }
        }

        public decimal GetEarnings(int days)
        {
            using (var db = new Context())
            {
                var salaries =
                    (from items in db.Items
                     join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                     join tags in db.Tags on itemTags.TagId equals tags.Id
                     where true
                     && tags.Name == "salary"
                     select items).ToList();

                var earliestAt = db.Items
                    .OrderBy(item => item.TransactedAt)
                    .First()
                    .TransactedAt;

                var latestAt = db.Items
                    .OrderByDescending(item => item.TransactedAt)
                    .First()
                    .TransactedAt;

                var amount = salaries.Aggregate(0.00M, (result, item) => result + item.Amount);
                var dateRange = latestAt - earliestAt;

                // Change sense because earnings are reported
                // as negative numbers
                return -1 * Convert.ToDecimal(Convert.ToDouble(amount) * days / dateRange.TotalDays);
            }
        }

        public List<Item> GetAllExpenses()
        {
            // TODO: what is the right tag name?
            const string creditCardPaymentTagName = "credit-card-payment";
            using (var db = new Context())
            {
                // var bankStatementItems = db.Items
                //     .Where(item => item.TransactedAt >= StartAt)
                //     .Where(item => item.TransactedAt < EndAt)
                //     .Where(item => item.Statement.Card.CardType == CardTypes.Bank)

                var bankStatementItems = db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(itemTags => itemTags.Tag)
                    .Where(item => item.Statement.Card.CardType == CardTypes.Bank)
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt);
                // var bankStatementItems =
                //     from items in db.Items
                //     join statements in db.Statements on items.StatementId equals statements.Id
                //     join cards in db.Cards on statements.CardId equals cards.Id
                //     join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                //     join tags in db.Tags on itemTags.TagId equals tags.Id
                //     where true
                //         && cards.CardType == CardTypes.Bank
                //         && items.TransactedAt >= StartAt
                //         && items.TransactedAt < EndAt
                //     select items;

                // var bankStatementPayments =
                //     from items in db.Items
                //     join statements in db.Statements on items.StatementId equals statements.Id
                //     join cards in db.Cards on statements.CardId equals cards.Id
                //     join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                //     join tags in db.Tags on itemTags.TagId equals tags.Id
                //     where true
                //         && tags.Name == creditCardPaymentTagName
                //         && cards.CardType == CardTypes.Bank
                //         && items.TransactedAt >= StartAt
                //         && items.TransactedAt < EndAt
                //     select items;

                // var bankStatementExpenses = db.Items
                //     .Where(item => !bankStatementPayments.Any(payment => payment.Id == item.Id));

                var creditCardItems = db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(itemTags => itemTags.Tag)
                    .Where(item => item.Statement.Card.CardType == CardTypes.Credit)
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt);
                // from items in db.Items
                // join statements in db.Statements on items.StatementId equals statements.Id
                // join cards in db.Cards on statements.CardId equals cards.Id
                // join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                // join tags in db.Tags on itemTags.TagId equals tags.Id
                // where true
                //     && cards.CardType == CardTypes.Credit
                //     && items.TransactedAt >= StartAt
                //     && items.TransactedAt < EndAt
                // select items;

                var totalBankStatementExpenses = bankStatementItems
                    .ToList()
                    .Where(item => !item.Tags.Any(tag => tag.Name == creditCardPaymentTagName));

                var totalCreditCardStatementExpenses = creditCardItems
                    .ToList()
                    .Where(item => !item.Tags.Any(tag => tag.Name == creditCardPaymentTagName));

                return totalBankStatementExpenses
                    .Concat(totalCreditCardStatementExpenses)
                    .ToList();
            }
        }
    }
}

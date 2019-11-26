using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

// TODO: Remove hardcoded filter tag names and replace with functions

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

        public IEnumerable<ValueTuple<Tag, Item>> GetItemsByTag()
        {
            using (var db = new Context())
            {
                return (
                     from t in db.Tags
                     join it in db.ItemTags on t.Id equals it.TagId
                     join i in db.Items on it.ItemId equals i.Id
                     where true
                         && i.At >= StartAt
                         && i.At < EndAt
                     select ValueTuple.Create<Tag, Item>(t, i)
                    ).ToList();
            }
        }

        public IDictionary<Tag, IList<Item>> GetAssetsByTag()
        {
            return GetItemByTag(true);
        }

        public IDictionary<Tag, IList<Item>> GetExpensesByTag()
        {
            return GetItemByTag(false);
        }

        public decimal GetExpenses()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Where(item => item.Statement.Card.CardType == CardTypes.Bank)
                    .Where(item => item.At >= StartAt)
                    .Where(item => item.At < EndAt)
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
                    .OrderBy(item => item.At)
                    .First()
                    .At;

                var latestAt = db.Items
                    .OrderByDescending(item => item.At)
                    .First()
                    .At;

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
                    .Where(item => item.At >= StartAt)
                    .Where(item => item.At < EndAt);
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
                    .Where(item => item.At >= StartAt)
                    .Where(item => item.At < EndAt);
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

        public IEnumerable<TagCost> GetTagExpenses()
        {
            return GetGroupedItemsByTags(false);
        }

        public IEnumerable<TagCost> GetTagAssets()
        {
            return GetGroupedItemsByTags(true);
        }

        private IEnumerable<TagCost> GetGroupedItemsByTags(bool isAsset)
        {
            Item[] items;
            using (var db = new Context())
            {
                items = db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => isAsset && item.IsCredit || !isAsset && item.IsDebit)
                    .Where(item => item.At >= StartAt)
                    .Where(item => item.At < EndAt)
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }

            return items
                .GroupBy(item => item.Tags.Select(tag => tag.Name).Join(", "))
                .Select(grouped => new TagCost(grouped.First().Tags, grouped));
        }

        private IDictionary<Tag, IList<Item>> GetItemByTag(bool isAsset)
        {
            List<ValueTuple<Tag, Item>> tagAndExpenses;
            using (var db = new Context())
            {
                tagAndExpenses = (
                    from t in db.Tags
                    join it in db.ItemTags on t.Id equals it.TagId
                    join i in db.Items on it.ItemId equals i.Id
                    where true
                        && i.At >= StartAt
                        && i.At < EndAt
                        && (isAsset && i.IsCredit || !isAsset && i.IsDebit)
                        && t.Name != "credit-card-payment"
                    select ValueTuple.Create<Tag, Item>(t, i)
                    ).ToList();
            }

            var results = new Dictionary<Tag, IList<Item>>();
            foreach (var tagAndExpense in tagAndExpenses)
            {
                if (results.ContainsKey(tagAndExpense.Item1))
                {
                    results[tagAndExpense.Item1].Add(tagAndExpense.Item2);
                }
                else
                {
                    results.Add(tagAndExpense.Item1, new List<Item> { tagAndExpense.Item2 });
                }
            }

            return results;
        }
    }
}

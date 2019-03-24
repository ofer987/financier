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

        public Dictionary<Tag, List<Item>> ItemsByGroup()
        {
            List<Item> items;
            using (var db = new Context())
            {
                items = db.Items
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt)
                    .Where(item => item.Amount > 0)
                    .ToList();
            }

            var itemsByTag = new Dictionary<Tag, List<Item>>();
            foreach (var item in items)
            {
                foreach (var tag in item.Tags)
                {
                    if (itemsByTag.TryGetValue(tag, out var taggedItems))
                    {
                        taggedItems.Add(item);
                    }
                    else
                    {
                        itemsByTag.Add(tag, new List<Item> { item });
                    }
                }
            }

            return itemsByTag;
        }

        public decimal GetExpenses()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.Statement)
                    .Where(item => item.Statement.Card.CardType == CardTypes.Credit)
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt)
                    .Where(item => item.Amount > 0)
                    .Aggregate(0.00M, (result, item) => result + item.Amount);
            }
        }

        public decimal GetEarnings()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt)
                    .Aggregate(0.00M, (result, item) => result + item.Amount);
            }
        }

        public decimal GetEarningsWithCcExpenses()
        {
            const string creditCardPaymentTagName = "creditcard-payment";
            using (var db = new Context())
            {
                // var bankStatementItems = db.Items
                //     .Where(item => item.TransactedAt >= StartAt)
                //     .Where(item => item.TransactedAt < EndAt)
                //     .Where(item => item.Statement.Card.CardType == CardTypes.Bank)

                var bankStatementItems =
                    from items in db.Items
                    join statements in db.Statements on items.StatementId equals statements.Id
                    join cards in db.Cards on statements.CardId equals cards.Id
                    join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                    join tags in db.Tags on itemTags.TagId equals tags.Id
                    where true
                        && tags.Name == creditCardPaymentTagName
                        && cards.CardType == CardTypes.Bank
                        && items.TransactedAt >= StartAt
                        && items.TransactedAt < EndAt
                    select items;

                var bankStatementPayments =
                    from items in db.Items
                    join statements in db.Statements on items.StatementId equals statements.Id
                    join cards in db.Cards on statements.CardId equals cards.Id
                    join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                    join tags in db.Tags on itemTags.TagId equals tags.Id
                    where true
                        && tags.Name == creditCardPaymentTagName
                        && cards.CardType == CardTypes.Bank
                        && items.TransactedAt >= StartAt
                        && items.TransactedAt < EndAt
                    select items;

                var bankStatementExpenses =
                    .Where(item => !bankStatementPayments.Any(payment => payment.Id == item.Id));

                var creditCardPayments =
                    from items in db.Items
                    join statements in db.Statements on items.StatementId equals statements.Id
                    join cards in db.Cards on statements.CardId equals cards.Id
                    join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                    join tags in db.Tags on itemTags.TagId equals tags.Id
                    where true
                        && tags.Name == creditCardPaymentTagName
                        && cards.CardType == CardTypes.Credit
                        && items.TransactedAt >= StartAt
                        && items.TransactedAt < EndAt
                    select items;
                
                var creditCardItems =
                    from items in db.Items
                    join statements in db.Statements on items.StatementId equals statements.Id
                    join cards in db.Cards on statements.CardId equals cards.Id
                    join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                    join tags in db.Tags on itemTags.TagId equals tags.Id
                    where true
                        && cards.CardType == CardTypes.Credit
                        && items.TransactedAt >= StartAt
                        && items.TransactedAt < EndAt
                    select items;

                var creditCardExpenses = creditCardItems
                    .Where(item => !creditCardPayments.Any(payment => payment.Id == item.Id));
            }
        }
    }
}

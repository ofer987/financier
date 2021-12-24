using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

// TODO: Remove hardcoded filter tag names and replace with functions

namespace Financier.Common.Expenses
{
    public class ProjectedCashFlow : DurationCashFlow
    {
        // public DateTime ProjectedAt { get; private set; }
        public decimal AverageCreditAmount { get; private set; }
        public decimal AverageDebitAmount { get; private set; }

        public ProjectedCashFlow(DateTime startAt, DateTime endAt, decimal threshold = DefaultThreshold)
        {
            StartAt = startAt;
            EndAt = endAt;
            // ProjectedAt = projectedAt;
            Threshold = threshold;

            Validate();
            Init();
        }

        public decimal GetProjectedCreditAt(DateTime at)
        {
            if (at <= this.EndAt)
            {
                throw new ArgumentException($"Can only project credit in the future, i.e., after {this.EndAt}", nameof(at));
            }

            return this.AverageCreditAmount;
        }

        public decimal GetProjectedDebitAt(DateTime at)
        {
            if (at <= this.EndAt)
            {
                throw new ArgumentException($"Can only project debit in the future, i.e., after {this.EndAt}", nameof(at));
            }

            return this.AverageDebitAmount;
        }

        protected void Validate()
        {
            if (this.StartAt.AddMonths(1) > this.EndAt)
            {
                throw new ArgumentException($"EndAt {this.EndAt} should be at least one month ahead of StartAt ({this.StartAt})", nameof(this.EndAt));
            }
        }

        protected override void Init()
        {
            SetCredits();
            SetDebits();

            SetAverageCredit();
            SetAverageDebit();
        }

        private void SetAverageCredit()
        {
            var monthsApart = 0
                + (this.EndAt.Year * 12 - this.StartAt.Year * 12)
                + (this.EndAt.Month - this.StartAt.Month);

            this.AverageCreditAmount = this.CreditAmountTotal / monthsApart;
        }

        private void SetAverageDebit()
        {
            var monthsApart = 0
                + (this.EndAt.Year * 12 - this.StartAt.Year * 12)
                + (this.EndAt.Month - this.StartAt.Month);

            this.AverageDebitAmount = this.DebitAmountTotal / monthsApart;
        }
    }
}

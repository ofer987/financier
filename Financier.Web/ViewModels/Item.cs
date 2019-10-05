using System;
using System.Text;
using System.Linq;

using Financier.Common.Extensions;

namespace Financier.Web.ViewModels
{
    public class Item
    {
        public Financier.Common.Expenses.Models.Item Model { get; }

        public Guid Id => Model.Id;
        public string Description => Model.Description;
        public decimal Amount => Model.Amount;
        public string AmountString => Amount.ToString("#0.00");

        private string tags = string.Empty;
        public string Tags
        {
            get
            {
                if (!tags.IsNullOrEmpty())
                {
                    return tags;
                }

                var sb = new StringBuilder();

                tags = Model.Tags
                    .Select(tag => tag.Name)
                    .Join();

                return tags;
            }
        }

        public string At
        {
            get
            {
                return Model.At.ToString("dd MMMM yyyy");
            }
        }

        public string Type
        {
            get
            {
                if (Model.IsCredit)
                {
                    return "Credit";
                }

                return "Debit";
            }
        }

        public Item(Financier.Common.Expenses.Models.Item model)
        {
            Model = model;
        }
    }
}

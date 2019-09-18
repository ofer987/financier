using System;
using System.Text;
using System.Linq;

using Financier.Common.Extensions;

namespace Financier.Web.ViewModels
{
    public class Item
    {
        public Financier.Common.Expenses.Models.Item Model { get;  }

        public Guid Id => Model.Id;
        public string Description => Model.Description;
        public decimal Amount => Model.Amount;

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

        public Item(Financier.Common.Expenses.Models.Item model)
        {
            Model = model;
        }
    }
}

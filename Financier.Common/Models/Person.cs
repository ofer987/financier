using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Models
{
    // Container for produdcts and sources of income
    public class Person
    {
        public List<IProduct> Products { get; }

        public List<Income.Base> IncomeSources { get; }

        public Person(IEnumerable<IProduct> products, IEnumerable<Income.Base> incomeSources)
        {
            Products = products.ToList();
            IncomeSources = incomeSources.ToList();
        }
    }
}

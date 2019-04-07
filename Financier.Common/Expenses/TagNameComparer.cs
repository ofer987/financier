using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class TagNameComparer : EqualityComparer<IEnumerable<Tag>>
    {
        public override bool Equals(IEnumerable<Tag> source, IEnumerable<Tag> target)
        {
            if (source.Count() != target.Count())
            {
                return false;
            }

            var sortedSource = source.OrderBy(tag => tag.Name).ToList();
            var sortedTarget = target.OrderBy(tag => tag.Name).ToList();
            for (var i = 0; i < sortedSource.Count; i += 1)
            {
                if (sortedSource[i] != sortedTarget[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode(IEnumerable<Tag> obj)
        {
            return obj
                .Select(item => item.Name)
                .OrderBy(name => name)
                .Aggregate(0, (result, name) => result + name.GetHashCode());
        }
    }
}

using System;
using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Tag CreateTag(string name)
        {
            return new Tag
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }

        public static Tag DanTag()
        {
            return new Tag
            {
                Id = Guid.NewGuid(),
                Name = "dan"
            };
        }

        public static Tag RonTag()
        {
            return new Tag
            {
                Id = Guid.NewGuid(),
                Name = "ron"
            };
        }

        public static Tag KerenTag()
        {
            return new Tag
            {
                Id = Guid.NewGuid(),
                Name = "keren"
            };
        }
    }
}

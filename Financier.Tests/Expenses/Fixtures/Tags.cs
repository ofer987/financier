using System;
using Financier.Common.Expenses.Models;

namespace Financier.Tests.Expenses.Fixtures
{
    public static class Tags
    {
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

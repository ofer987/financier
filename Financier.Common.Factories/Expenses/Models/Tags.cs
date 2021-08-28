using System;
using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Tag NewTag(string name)
        {
            return new Tag
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }

        public static Tag CreateTag(string name)
        {
            var tag = NewTag(name);

            using (var db = new Context())
            {
                db.Tags.Add(tag);
                db.SaveChanges();
            }

            return tag;
        }

        public static Tag DanTag()
        {
            return NewTag("dan");
        }

        public static Tag RonTag()
        {
            return NewTag("ron");
        }

        public static Tag KerenTag()
        {
            return NewTag("keren");
        }
    }
}

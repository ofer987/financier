using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common;

namespace Financier.Cli.Listings
{
    class Program
    {
        static void Main(string[] args)
        {
            Context.Environment = Environments.Dev;
            Console.WriteLine("Hello World!");

            using (var db = new Context())
            {
                var selectedStatement = db.Statements
                    .Include(stmt => stmt.Items)
                    .First(stmt => stmt.Id == Guid.Parse(args[0]));

                Console.WriteLine(selectedStatement);
            }
        }
    }
}

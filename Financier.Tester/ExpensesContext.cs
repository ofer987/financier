using Financier.Common.Models.Expenses;
using Microsoft.EntityFrameworkCore;

namespace Financier.Tester
{
    public class ExpensesContext : DbContext
    {
        public DbSet<Item> Items { get; }

        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Financier.db");
        }
    }
}

using Financier.Common.Models.Expenses;
using Microsoft.EntityFrameworkCore;

namespace Financier.Tester
{
    public class ExpensesContext : DbContext
    {
        public DbSet<Item> Items { get; }

        // public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options)
        // {
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasKey(item => item.Id);

            modelBuilder.Entity<Item>()
                .HasOne(item => item.Statement)
                .WithMany(statement => statement.Items)
                .HasForeignKey(item => item.StatementId);

            modelBuilder.Entity<Statement>()
                .HasKey(statement => statement.Id);

            modelBuilder.Entity<Card>()
                .HasKey(card => card.Id);

            modelBuilder.Entity<Statement>()
                .HasOne(statement => statement.Card)
                .WithMany(card => card.Statements)
                .HasForeignKey(statement => statement.CardId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch(Program.Environment)
            {
                case Environments.Development:
                default:
                    optionsBuilder.UseSqlite("Data Source=Financier.db");
                    break;
            }
        }
    }
}

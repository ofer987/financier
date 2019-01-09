using Financier.Common.Models.Expenses;
using Microsoft.EntityFrameworkCore;

namespace Financier.Common
{
    public class ExpensesContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }

        public DbSet<Statement> Statements { get; set; }

        public DbSet<Item> Items { get; set; }

        public ExpensesContext()
        {
        }

        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options)
        {
        }

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

            modelBuilder.Entity<Card>()
                .HasMany(card => card.Statements)
                .WithOne(statement => statement.Card)
                .HasForeignKey(statement => statement.CardId);

            modelBuilder.Entity<Statement>()
                .HasOne(statement => statement.Card)
                .WithMany(card => card.Statements)
                .HasForeignKey(statement => statement.CardId);

            modelBuilder.Entity<Statement>()
                .HasMany(statement => statement.Items)
                .WithOne(item => item.Statement)
                .HasForeignKey(item => item.StatementId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=/Users/ofer987/work/Financier/Financier.Tests/Financier.db");
            }
        }
    }
}

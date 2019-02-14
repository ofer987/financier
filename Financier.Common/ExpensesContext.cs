using Financier.Common.Models.Expenses;
using Microsoft.EntityFrameworkCore;
// using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Financier.Common
{
    public enum Environments { Dev = 0, Test, Production }

    public class ExpensesContext : DbContext
    {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new ILoggerProvider[] { new ConsoleLoggerProvider((_, __) => true, true )});

        public DbSet<Card> Cards { get; set; }

        public DbSet<Statement> Statements { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ItemTag> ItemTags { get; set; }

        public static Environments Environment = Environments.Dev;

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

            modelBuilder.Entity<Item>()
                .HasMany(item => item.ItemTags)
                .WithOne(it => it.Item)
                .HasForeignKey(it => it.ItemId);

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

            modelBuilder.Entity<Tag>()
                .HasIndex(tag => tag.Name)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasMany(tag => tag.ItemTags)
                .WithOne(it => it.Tag)
                .HasForeignKey(it => it.TagId);

            modelBuilder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });

            //
            // modelBuilder.Entity<Statement>()
            //     .HasMany(statement => statement.Items)
            //     .WithOne(item => item.Statement)
            //     .HasForeignKey(item => item.StatementId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // switch (Environment)
            // {
            //     case Environments.Test:
            //         var connection = new SqliteConnection("DataSource=:memory:");
            //         connection.Open();
            //         optionsBuilder.UseSqlite("DataSource=:memory:");
            //         break;
            //     default:
            //         break;
            // }

            if (!optionsBuilder.IsConfigured)
            {
                // optionsBuilder
                //     .UseLoggerFactory(MyLoggerFactory)
                //     .EnableSensitiveDataLogging();
                    // .UseSqlite("Data Source=/Users/ofer987/work/Financier/Financier.Tests/Financier.db");

                switch (Environment)
                {
                    case Environments.Dev:
                        optionsBuilder.UseNpgsql("Host=localhost;Database=financier;Username=ofer987");
                        break;
                    case Environments.Test:
                        optionsBuilder.UseNpgsql("Host=localhost;Database=financier_tests;Username=ofer987");
                        break;
                    case Environments.Production:
                        optionsBuilder.UseNpgsql("Host=localhost;Database=financier;Username=ofer987");
                        break;
                }

            }
            else
            {
                // optionsBuilder
                //     .UseLoggerFactory(MyLoggerFactory);
            }
        }
    }
}

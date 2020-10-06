using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public enum Environments { Dev = 0, Test, Production }

    public class Context : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }

        public static Environments Environment = Environments.Dev;

        public static void Clean()
        {
            using (var db = new Context())
            {
                db.Items.RemoveRange(db.Items);
                db.Statements.RemoveRange(db.Statements);
                db.Cards.RemoveRange(db.Cards);
                db.Accounts.RemoveRange(db.Accounts);
                db.Tags.RemoveRange(db.Tags);
                db.ItemTags.RemoveRange(db.ItemTags);

                db.SaveChanges();
            }
        }

        public Context() : base()
        {
        }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>()
                .HasKey(item => item.Name);

            builder.Entity<Account>()
                .HasMany(account => account.Cards)
                .WithOne(card => card.Owner)
                .HasForeignKey(card => card.AccountName);

            builder.Entity<Card>()
                .HasOne(card => card.Owner)
                .WithMany(owner => owner.Cards)
                .HasPrincipalKey(owner => owner.Name)
                .HasForeignKey(card => card.AccountName);

            builder.Entity<Item>()
                .HasKey(item => item.Id);

            builder.Entity<Item>()
                .HasOne(item => item.Statement)
                .WithMany(statement => statement.Items)
                .HasForeignKey(item => item.StatementId);

            builder.Entity<Item>()
                .HasMany(item => item.ItemTags)
                .WithOne(it => it.Item)
                .HasForeignKey(it => it.ItemId);

            builder.Entity<Statement>()
                .HasKey(statement => statement.Id);

            builder.Entity<Card>()
                .HasKey(card => card.Id);

            // builder.Entity<Card>()
            //     .HasKey(card => card.Number);

            builder.Entity<Card>()
                .HasMany(card => card.Statements)
                .WithOne(statement => statement.Card)
                .HasForeignKey(statement => statement.CardId);

            builder.Entity<Statement>()
                .HasOne(statement => statement.Card)
                .WithMany(card => card.Statements)
                .HasForeignKey(statement => statement.CardId);

            builder.Entity<Statement>()
                .HasIndex(statement => new { statement.CardId, statement.PostedAt })
                .IsUnique();

            builder.Entity<Item>()
                .HasIndex(item => new { item.StatementId, item.ItemId })
                .IsUnique();

            builder.Entity<Tag>()
                .HasIndex(tag => tag.Name)
                .IsUnique();

            builder.Entity<Tag>()
                .HasMany(tag => tag.ItemTags)
                .WithOne(it => it.Tag)
                .HasForeignKey(it => it.TagId);

            builder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
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
            }
        }
    }
}

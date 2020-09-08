using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public enum Environments { Dev = 0, Test, Production }

    public class Context : IdentityDbContext
    {
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
                db.Tags.RemoveRange(db.Tags);
                db.ItemTags.RemoveRange(db.ItemTags);

                db.SaveChanges();
            }
        }

        public Context()
        {
        }

        public Context(DbContextOptions<Context> options) : base(options)
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

            modelBuilder.Entity<Statement>()
                .HasIndex(statement => new { statement.CardId, statement.PostedAt })
                .IsUnique();

            modelBuilder.Entity<Item>()
                .HasIndex(item => new { item.StatementId, item.ItemId })
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(tag => tag.Name)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasMany(tag => tag.ItemTags)
                .WithOne(it => it.Tag)
                .HasForeignKey(it => it.TagId);

            modelBuilder.Entity<ItemTag>()
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

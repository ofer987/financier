﻿// <auto-generated />
using System;
using Financier.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Financier.Common.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20190221015418_Add_ItemId_To_Item")]
    partial class Add_ItemId_To_Item
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Financier.Common.Expenses.Models.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Number")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Expenses_Cards");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("ItemId");

                    b.Property<DateTime>("PostedAt");

                    b.Property<Guid>("StatementId");

                    b.Property<DateTime>("TransactedAt");

                    b.HasKey("Id");

                    b.HasIndex("StatementId", "ItemId")
                        .IsUnique();

                    b.ToTable("Expenses_Items");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.ItemTag", b =>
                {
                    b.Property<Guid>("ItemId");

                    b.Property<Guid>("TagId");

                    b.HasKey("ItemId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("Expenses_ItemTags");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Statement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CardId");

                    b.Property<DateTime>("PostedAt");

                    b.HasKey("Id");

                    b.HasIndex("CardId", "PostedAt")
                        .IsUnique();

                    b.ToTable("Expenses_Statements");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Expenses_Tags");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Item", b =>
                {
                    b.HasOne("Financier.Common.Expenses.Models.Statement", "Statement")
                        .WithMany("Items")
                        .HasForeignKey("StatementId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.ItemTag", b =>
                {
                    b.HasOne("Financier.Common.Expenses.Models.Item", "Item")
                        .WithMany("ItemTags")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Financier.Common.Expenses.Models.Tag", "Tag")
                        .WithMany("ItemTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Statement", b =>
                {
                    b.HasOne("Financier.Common.Expenses.Models.Card", "Card")
                        .WithMany("Statements")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

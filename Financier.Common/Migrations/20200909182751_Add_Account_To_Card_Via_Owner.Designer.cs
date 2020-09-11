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
    [Migration("20200909182751_Add_Account_To_Card_Via_Owner")]
    partial class Add_Account_To_Card_Via_Owner
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Financier.Common.Expenses.Models.Account", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.ToTable("Expenses_Accounts");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CardType")
                        .HasColumnType("integer");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AccountName");

                    b.ToTable("Expenses_Cards");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ItemId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("PostedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("StatementId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("TransactedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("StatementId", "ItemId")
                        .IsUnique();

                    b.ToTable("Expenses_Items");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.ItemTag", b =>
                {
                    b.Property<Guid>("ItemId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uuid");

                    b.HasKey("ItemId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("Expenses_ItemTags");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Statement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("PostedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CardId", "PostedAt")
                        .IsUnique();

                    b.ToTable("Expenses_Statements");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Expenses_Tags");
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Card", b =>
                {
                    b.HasOne("Financier.Common.Expenses.Models.Account", "Owner")
                        .WithMany("Cards")
                        .HasForeignKey("AccountName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Item", b =>
                {
                    b.HasOne("Financier.Common.Expenses.Models.Statement", "Statement")
                        .WithMany("Items")
                        .HasForeignKey("StatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.ItemTag", b =>
                {
                    b.HasOne("Financier.Common.Expenses.Models.Item", "Item")
                        .WithMany("ItemTags")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Financier.Common.Expenses.Models.Tag", "Tag")
                        .WithMany("ItemTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Financier.Common.Expenses.Models.Statement", b =>
                {
                    b.HasOne("Financier.Common.Expenses.Models.Card", "Card")
                        .WithMany("Statements")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
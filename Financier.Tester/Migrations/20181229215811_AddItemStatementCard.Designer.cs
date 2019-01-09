﻿// <auto-generated />
using System;
using Financier.Tester;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Financier.Tester.Migrations
{
    [DbContext(typeof(ExpensesContext))]
    [Migration("20181229215811_AddItemStatementCard")]
    partial class AddItemStatementCard
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity("Financier.Common.Models.Expenses.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RealId");

                    b.HasKey("Id");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("Financier.Common.Models.Expenses.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("Description");

                    b.Property<DateTime>("PostedAt");

                    b.Property<Guid>("StatementId");

                    b.Property<DateTime>("TransactedAt");

                    b.HasKey("Id");

                    b.HasIndex("StatementId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Financier.Common.Models.Expenses.Statement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CardId");

                    b.Property<DateTime>("PostedAt");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("Statement");
                });

            modelBuilder.Entity("Financier.Common.Models.Expenses.Item", b =>
                {
                    b.HasOne("Financier.Common.Models.Expenses.Statement", "Statement")
                        .WithMany("Items")
                        .HasForeignKey("StatementId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Financier.Common.Models.Expenses.Statement", b =>
                {
                    b.HasOne("Financier.Common.Models.Expenses.Card", "Card")
                        .WithMany("Statements")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
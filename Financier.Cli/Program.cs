using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using McMaster.Extensions.CommandLineUtils;

using Financier.Common;
using Financier.Common.Extensions;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Cli
{
    public class Program
    {
        public List<Tagging> Taggings = new List<Tagging>
        {
            new Tagging(@"groceries", new[] { "food", "groceries" }),
            new Tagging(@"freshco", new[] { "food", "groceries" }),
            new Tagging(@"nofrills", new[] { "food", "groceries" }),
            new Tagging(@"metro", new[] { "food", "groceries" }),
            new Tagging(@"sababa fine foods", new[] { "food", "groceries" }),
            new Tagging(@"farm boy", new[] { "food", "groceries" }),
            new Tagging(@"iga express", new[] { "food", "groceries" }),
            new Tagging(@"instacart", new[] { "food", "groceries" }),
            new Tagging(@"airbnb", new[] { "lodging", "trip" }),
            new Tagging(@"ikea", new[] { "home-improvement" }),
            new Tagging(@"pilot coffee", new[] { "coffee" }),
            new Tagging(@"pamenar", new[] { "coffee" }),
            new Tagging(@"applevale orchards", new[] { "trip", "family-activity" }),
            new Tagging(@"cafe des amis", new[] { "coffee" }),
            new Tagging(@"cocoa latte", new[] { "coffee" }),
            new Tagging(@"cinnaholic", new[] { "coffee" }),
            new Tagging(@"cafe lali", new[] { "coffee" }),
            new Tagging(@"uber *trip", new[] { "transportation", "taxi" }),
            new Tagging(@"canada computers", new[] { "computer", "accessories" }),
            new Tagging(@"IQ Food", new[] { "coffee" }),
            new Tagging(@"the library specia", new[] { "coffee" }),
            new Tagging(@"tim horton's", new[] { "coffee" }),
            new Tagging(@"neon", new[] { "clothing" }),
            new Tagging(@"paddington's pump", new[] { "food", "meal" }),
            new Tagging(@"uber eats", new[] { "food", "meal" }),
            new Tagging(@"the oxley", new[] { "food", "meal" }),
            new Tagging(@"aldo's restaurant", new[] { "food", "meal" }),
            new Tagging(@"digital ocean", new[] { "internet", "computer" }),
            new Tagging(@"github", new[] { "internet", "computer" }),
            new Tagging(@"google", new[] { "internet", "computer" }),
            new Tagging(@"teranet", new[] { "health" }),
            new Tagging(@"healthcare", new[] { "health" }),
            new Tagging(@"ecco", new[] { "shoes" }),
            new Tagging(@"zooland", new[] { "indoor-playground", "alexander", "benjamin", "constantin" }),
            new Tagging(@"rakuten", new[] { "books" }),
            new Tagging(@"apple\.com/bill", new[] { "internet", "computer" }),
            new Tagging(@"scaddabush", new[] { "food", "meal" }),
            new Tagging(@"bagel", new[] { "groceries" }),
            new Tagging(@"pharmac", new[] { "health", "drugs" }),
            new Tagging(@"staples", new[] { "alexander", "benjamin", "school-supplies" }),
            new Tagging(@"pizza", new[] { "food", "meal" }),
            new Tagging(@"deja vu", new[] { "food", "meal" }),
            new Tagging(@"king slice", new[] { "food", "meal" }),
            new Tagging(@"jerk king", new[] { "food", "meal" }),
            new Tagging(@"swadish grill", new[] { "food", "meal" }),
            new Tagging(@"oshkosh", new[] { "clothing", "alexander", "benjamin", "constantin" }),
            new Tagging(@"stm", new[] { "transportation", "public-transportation" }),
            new Tagging(@"st\. michael's hospita", new[] { "charity" }),
            new Tagging(@"uber bv", new[] { "tip", "transportation" }),
            new Tagging(@"cdn tire store", new[] { "home-improvement" }),
            new Tagging(@"dairy queen", new[] { "food", "dessert" }),
            new Tagging(@"baskin robbins", new[] { "food", "dessert" }),
            new Tagging(@"indigo", new[] { "books" }),
        };

        [OptionAttribute("-c|--credit-cards", CommandOptionType.SingleOrNoValue)]
        public (bool HasValue, string Value) CreditCardStatementsPath { get; }

        [OptionAttribute("-b|--bank-cards", CommandOptionType.SingleOrNoValue)]
        public (bool HasValue, string Value) BankStatementsPath { get; }

        [OptionAttribute("-a|--auto-assign", CommandOptionType.NoValue)]
        public bool IsAutoAssign { get; }

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            Context.Environment = Environments.Dev;

            foreach (var file in GetCreditCardStatements())
            {
                new CreditCardStatementFile(file).Import();
            }

            foreach (var file in GetBankStatements())
            {
                new BankStatementFile(file).Import();
            }

            Console.WriteLine("Processing Items");
            var newItems = Item.GetAllNewItems();
            var i = 0;
            foreach (var item in newItems)
            {
                i += 1;

                Console.WriteLine($"Processing Item ({i} of {newItems.Length}):");
                Console.WriteLine(item);

                var tagManager = new TagManager(item);
                var similarTags = tagManager.GetSimilarTagsByDescription();

                if (similarTags.Length > 0 && IsAutoAssign)
                {
                    tagManager.AddTags(similarTags);
                }
                else if (similarTags.Length > 0)
                {
                    // TODO: Place in its own function
                    var repeat = true;
                    do
                    {
                        Console.WriteLine($"Found similar tags:");
                        Console.WriteLine(similarTags.Join("\n"));
                        Console.WriteLine();
                        Console.WriteLine("Use them? (Y[es]/n[o])");
                        var response = Console.ReadLine();

                        repeat = false;
                        switch (response.ToLower().Trim())
                        {
                            case "":
                            case "y":
                            case "yes":
                                tagManager.AddTags(similarTags);
                                break;
                            case "n":
                            case "no":
                                Console.WriteLine("Input tags:");
                                var newTagsString = ReadNewTags();
                                Console.WriteLine();

                                var newTags = TagManager.FindOrCreateTags(newTagsString);
                                tagManager.AddTags(newTags);
                                break;
                            default:
                                repeat = true;
                                break;
                        }
                    } while (repeat);
                }
                else
                {
                    Console.WriteLine("Input tags:");
                    var newTagsString = ReadNewTags();
                    Console.WriteLine();

                    var newTags = TagManager.FindOrCreateTags(newTagsString);
                    tagManager.AddTags(newTags);
                }
            }
        }

        public static string GetCreditCardStatementsPath(IReadOnlyList<string> args)
        {
            return args[0];
        }

        public FileInfo[] GetBankStatements()
        {
            if (!BankStatementsPath.HasValue)
            {
                return new FileInfo[0];
            }

            Console.WriteLine("Getting Bank Statements");
            return Statements.GetCsvFiles(BankStatementsPath.Value);
        }

        public FileInfo[] GetCreditCardStatements()
        {
            if (!CreditCardStatementsPath.HasValue)
            {
                return new FileInfo[0];
            }

            Console.WriteLine("Getting Credit Card Statements");
            return Statements.GetCsvFiles(CreditCardStatementsPath.Value);
        }

        public static string ReadNewTags()
        {
            return Console.ReadLine();
        }
    }
}

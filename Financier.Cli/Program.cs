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
                // Commented-out for now
                // TODO: group cards in directories by name of account, for example,
                // Dan's cards will be nested in the dan@ofer.to
                // new CreditCardStatementFile(file).Import();
            }

            foreach (var file in GetBankStatements())
            {
                // Commented-out for now
                // TODO: group cards in directories by name of account, for example,
                // Dan's cards will be nested in the dan@ofer.to
                // new BankStatementFile(file).Import();
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

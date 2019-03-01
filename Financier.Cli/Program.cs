using System;
using System.Collections.Generic;
using System.Linq;
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

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            Context.Environment = Environments.Dev;

            foreach (var file in GetCreditCardStatements())
            {
                new CreditCardStatementImporter().Import(file.GetPostedAt(), file.GetFileStream());
            }

            foreach (var file in GetBankStatements())
            {
                new BankStatementImporter().Import(file.GetPostedAt(), file.GetFileStream());
            }

            Console.WriteLine("Processing Items");
            foreach (var item in Item.GetAll())
            {
                Console.WriteLine();
                Console.WriteLine("Processing Item:");
                Console.WriteLine(item);

                var tagManager = new TagManager(item);
                var similarTags = tagManager.GetSimilarTagsByDescription();

                if (similarTags.Length > 0)
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
                    var newTags = TagManager.FindOrCreateTags(newTagsString);
                    tagManager.AddTags(newTags);
                }
            }
        }

        public static string GetCreditCardStatementsPath(IReadOnlyList<string> args)
        {
            return args[0];
        }

        public StatementFile[] GetBankStatements()
        {
            if (!BankStatementsPath.HasValue)
            {
                return new StatementFile[0];
            }

            Console.WriteLine("Getting Bank Statements");
            return StatementFile.GetCsvFiles(BankStatementsPath.Value);
        }

        public StatementFile[] GetCreditCardStatements()
        {
            if (!CreditCardStatementsPath.HasValue)
            {
                return new StatementFile[0];
            }

            Console.WriteLine("Getting Credit Card Statements");
            return StatementFile.GetCsvFiles(CreditCardStatementsPath.Value);
        }

        public static string ReadNewTags()
        {
            return Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common;
using Financier.Common.Extensions;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Context.Environment = Environments.Dev;
            var csvPath = GetCreditCardStatementsPath(args);
            
            // Context.Clean();

            // var statements = new StatementFile(args[0]).GetAll();


            // var postedAt = GetPostedAt(args[0]);
            // var stream = System.IO.File.OpenRead(GetStatementPath(args));

            // var importer = new StatementImporter();
            // var statement = importer.Import(postedAt, stream);
            //
            // Console.WriteLine(statement.Items.Count);
            var files = StatementFile.GetCsvFiles(GetCreditCardStatementsPath(args));
            foreach (var file in files)
            {
                new CreditCardStatementImporter().Import(file.GetPostedAt(), file.GetFileStream());
            }

            foreach (var file in StatementFile.GetCsvFiles(GetBankStatementsPath(args)))
            {
                new BankStatementImporter().Import(file.GetPostedAt(), file.GetFileStream());
            }

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

        public static string GetBankStatementsPath(IReadOnlyList<string> args)
        {
            return args[1];
        }

        public static string ReadNewTags()
        {
            return Console.ReadLine();
        }

        // public FileInfo[] GetStatements(string path)
        // {
        //     return new DirectoryInfo(path).GetFiles("*.csv");
        // }
        //
        // public Statement ParseStatement(FileInfo file)
        // {
        //     return new StatementImporter().Import(
        //         Guid.NewGuid(),
        //         GetPostedAt(file.Name),
        //         file.OpenRead()
        //     );
        // }
        //
        // public void SetTagsForItem(Item item)
        // {
        // }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
            Context.Clean();

            var postedAt = GetPostedAt(args[0]);
            var stream = System.IO.File.OpenRead(GetStatementPath(args));

            var importer = new StatementImporter();
            var statement = importer.Import(postedAt, stream);

            Console.WriteLine(statement.Items.Count);
            foreach (var item in statement.Items)
            {
                Console.WriteLine(item);

                var tagManager = new TagManager(item);
                var similarTags = tagManager.GetSimilarTagsByDescription();

                if (similarTags.Length > 0)
                {
                    // TODO: Place in its own function
                    var repeat = true;
                    do
                    {
                        Console.WriteLine($"Found similar tags: {similarTags.Join(", ")}");
                        Console.WriteLine("Use them? (Y[es]/n[o])");
                        var response = Console.ReadLine();

                        repeat = false;
                        switch (response.ToLower().Trim())
                        {
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


                // Console.WriteLine($"Input Tags for {item}");
                // importer.FindOrCreateTags(Console.ReadLine());
            }
        }

        public static DateTime GetPostedAt(string fileName)
        {
            return DateTime.ParseExact(fileName, "yyyyMMdd", null);
        }

        public static string GetStatementPath(IReadOnlyList<string> args)
        {
            return args[1];
        }

        public static string ReadNewTags()
        {
            return Console.ReadLine();
        }
    }
}

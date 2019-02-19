using System;
using System.Collections.Generic;

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
                var newTagsString = string.Empty;

                Tag[] selectedTags;
                if (similarTags.Length > 0)
                {
                    // TODO: Place in its own function
                    var repeat = false;
                    while (!repeat)
                    {
                        repeat = false;
                        Console.WriteLine($"Found similar tags: {similarTags.Join(", ")}");
                        Console.WriteLine("Use them? (Y[es]/n[o])");
                        var response = Console.ReadLine();

                        switch (response.ToLower())
                        {
                            case "y":
                            case "yes":
                                selectedTags = similarTags;
                                break;
                            case "n":
                            case "no":
                                newTagsString = ReadNewTags();
                                break;
                            default:
                                repeat = true;
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Input tags:");
                    newTagsString = ReadNewTags();
                }

                var newTags = TagManager.FindOrCreateTags(newTagsString);
                tagManager.AddTags(newTags);

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

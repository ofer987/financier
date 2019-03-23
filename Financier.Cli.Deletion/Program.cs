using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

using Financier.Common;

namespace Financier.Cli.Deletion
{
    public class Program
    {
        [OptionAttribute("-t|--type", CommandOptionType.SingleValue)]
        public string _modelType { get; }
        public string ModelType => (_modelType ?? string.Empty).Trim().ToLower();

        [OptionAttribute("-n|--name", CommandOptionType.SingleValue)]
        private string _modelName { get; }
        public string ModelName => (_modelName ?? string.Empty).Trim();

        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            try
            {
                switch (ModelType)
                {
                    case "card":
                        using (var db = new Context())
                        {
                            db.Cards
                                .First(card => card.Id == Guid.Parse(ModelName))
                                .Delete();
                        }
                        break;
                    case "statement":
                        using (var db = new Context())
                        {
                            db.Statements
                                .First(statement => statement.Id == Guid.Parse(ModelName))
                                .Delete();
                        }
                        break;
                    default:
                        Console.WriteLine($"Invalid value {ModelType}");
                        Console.WriteLine("Valid values:");
                        Console.WriteLine("Card");
                        Console.WriteLine("Statement");
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }
        }
    }
}

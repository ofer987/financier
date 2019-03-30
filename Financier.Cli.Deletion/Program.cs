using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

using Financier.Common;

namespace Financier.Cli.Deletion
{
    public class Program
    {
        [OptionAttribute("-m|--model", CommandOptionType.SingleValue)]
        public string _modelType { get; }
        public string ModelType => (_modelType ?? string.Empty).Trim().ToLower();

        [OptionAttribute("-n|--name", CommandOptionType.SingleValue)]
        private string _modelName { get; }
        public string ModelName => (_modelName ?? string.Empty).Trim();

        [OptionAttribute("-t|--target-name", CommandOptionType.SingleOrNoValue)]
        public (bool HasValue, string Value) _targetName { get; }
        public string TargetName => (_targetName.Value ?? string.Empty).Trim();

        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            try
            {
                if (_targetName.HasValue)
                {
                    Rename();
                }
                else
                {
                    Delete();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }
        }

        private void Rename()
        {
            switch (ModelType)
            {
                case "tag":
                    using (var db = new Context())
                    {
                        db.Tags
                            .First(tag => tag.Name == ModelName)
                            .Rename(TargetName);
                    }
                    break;
                default:
                    Console.WriteLine($"Invalid value {ModelType}");
                    Console.WriteLine("Valid values:");
                    Console.WriteLine("Tag");
                    break;
            }
        }

        private void Delete()
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
                case "tag":
                    using (var db = new Context())
                    {
                        db.Tags
                            .First(tag => tag.Name == ModelName)
                            .Delete();
                    }
                    break;
                default:
                    Console.WriteLine($"Invalid value {ModelType}");
                    Console.WriteLine("Valid values:");
                    Console.WriteLine("Card");
                    Console.WriteLine("Statement");
                    Console.WriteLine("Tag");
                    break;
            }
        }
    }
}

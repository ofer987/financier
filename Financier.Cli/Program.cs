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
        public List<ItemTagger> Taggings = new List<ItemTagger>
        {
            new ItemTagger(@"groceries", new[] { "food", "groceries" }),
            new ItemTagger(@"sobeys", new[] { "food", "groceries" }),
            new ItemTagger(@"freshco", new[] { "food", "groceries" }),
            new ItemTagger(@"nofrills", new[] { "food", "groceries" }),
            new ItemTagger(@"metro", new[] { "food", "groceries" }),
            new ItemTagger(@"sababa fine foods", new[] { "food", "groceries" }),
            new ItemTagger(@"farm boy", new[] { "food", "groceries" }),
            new ItemTagger(@"iga express", new[] { "food", "groceries" }),
            new ItemTagger(@"instacart", new[] { "food", "groceries" }),
            new ItemTagger(@"longo's", new[] { "food", "groceries" }),
            new ItemTagger(@"angelas deli", new[] { "food", "groceries" }),
            new ItemTagger(@"high park concession", new[] { "food" }),
            new ItemTagger(@"airbnb", new[] { "lodging", "trip" }),
            new ItemTagger(@"ikea", new[] { "home-improvement" }),
            new ItemTagger(@"arveen express cafe", new[] { "coffee" }),
            new ItemTagger(@"think coffee", new[] { "coffee" }),
            new ItemTagger(@"macaron and m", new[] { "coffee" }),
            new ItemTagger(@"zaza", new[] { "coffee" }),
            new ItemTagger(@"tommy cafe", new[] { "coffee" }),
            new ItemTagger(@"hotblack", new[] { "coffee" }),
            new ItemTagger(@"third wave coffee", new[] { "coffee" }),
            new ItemTagger(@"pilot coffee", new[] { "coffee" }),
            new ItemTagger(@"outpost coffee", new[] { "coffee" }),
            new ItemTagger(@"tim hortons", new[] { "coffee" }),
            new ItemTagger(@"tbg garden cafe", new[] { "coffee", "food" }),
            new ItemTagger(new [] { @"tim hortons", @"strange love coffee", "jimmy's coffee" }, new[] { "coffee" }),
            new ItemTagger(@"goldstruck", new[] { "coffee" }),
            new ItemTagger(@"the social blend", new[] { "coffee" }),
            new ItemTagger(@"postal service", new[] { "mail" }),
            new ItemTagger(@"tdsb", new[] { "school" }),
            new ItemTagger(@"lego brand", new[] { "toys", "children" }),
            new ItemTagger(@"lego  yorkdale", new[] { "toys", "children" }),
            new ItemTagger(@"toys r us", new[] { "toys", "children" }),
            new ItemTagger(@"sephora", new[] { "makeup", "edith" }),
            new ItemTagger(@"citizenship \& imm", new[] { "edith", "citizenship" }),
            new ItemTagger(new [] { @"cash advance fee adj", "interest advances", "annual card fee waiver" }, new[] { "fees" }),
            new ItemTagger(new [] { @"loblaws", @"lucky moose food mart" }, new[] { "groceries", "food" }),
            new ItemTagger(@"marche le richmond", new[] { "bakery", "food", "coffee", "trip", "montreal" }),
            new ItemTagger(@"mike's fish market", new[] { "fish", "groceries", "food" }),
            new ItemTagger(new [] { @"st urbain", @"cosenza bakery" }, new[] { "bakery", "food" }),
            new ItemTagger(new [] { @"belle's bakery", "blackbird" }, new[] { "bakery", "food" }),
            new ItemTagger(@"the box donut", new[] { "bakery", "food", "dessert" }),
            new ItemTagger(@"gelato simply", new[] { "ice cream", "dessert" }),
            new ItemTagger(@"pamenar", new[] { "coffee" }),
            new ItemTagger(@"cafe myriade", new[] { "coffee", "trip", "montreal" }),
            new ItemTagger(@"applevale orchards", new[] { "trip", "family-activity" }),
            new ItemTagger(@"cafe des amis", new[] { "coffee", "trip", "montreal" }),
            new ItemTagger(@"cocoa latte", new[] { "coffee" }),
            new ItemTagger(@"cinnaholic", new[] { "coffee" }),
            new ItemTagger(@"cafe lali", new[] { "coffee", "trip", "montreal" }),
            new ItemTagger(@"uber *trip", new[] { "transportation", "taxi" }),
            new ItemTagger(@"cr7 shawarma bbq", new[] { "food", "meal" }),
            new ItemTagger(@"ubr*", new[] { "food", "meal" }),
            new ItemTagger(new [] { @"szechuan express", @"kid lee", @"stock tc", @"grenadier cafe", @"flo's diner", @"mr\. greek mediter" }, new[] { "food", "meal" }),
            new ItemTagger(@"saffron spice kitchen", new[] { "food", "meal" }),
            new ItemTagger(@"hy's steakhouse*", new[] { "food", "meal" }),
            new ItemTagger(@"duke of york", new[] { "food", "meal" }),
            new ItemTagger(@"canada computers", new[] { "computer", "accessories" }),
            new ItemTagger(@"IQ Food", new[] { "coffee" }),
            new ItemTagger(@"shy coffee", new[] { "coffee" }),
            new ItemTagger(@"the library specia", new[] { "coffee" }),
            new ItemTagger(@"tim horton's", new[] { "coffee" }),
            new ItemTagger(@"famous player", new[] { "movies" }),
            new ItemTagger(new [] { @"neon", @"la vie en rose", @"dear born baby" }, new[] { "clothing" }),
            new ItemTagger(@"paddington's pump", new[] { "food", "meal" }),
            new ItemTagger(@"uber eats", new[] { "food", "meal" }),
            new ItemTagger(@"the oxley", new[] { "food", "meal" }),
            new ItemTagger(@"aldo's restaurant", new[] { "food", "meal" }),
            new ItemTagger(@"digital ocean", new[] { "internet", "computer" }),
            new ItemTagger(@"github", new[] { "internet", "computer" }),
            new ItemTagger(@"google", new[] { "internet", "computer" }),
            new ItemTagger(@"teranet", new[] { "health" }),
            new ItemTagger(@"healthcare", new[] { "health" }),
            new ItemTagger(@"ecco", new[] { "shoes" }),
            new ItemTagger(@"zooland", new[] { "indoor-playground", "alexander", "benjamin", "constantin" }),
            new ItemTagger(@"rakuten", new[] { "books" }),
            new ItemTagger(@"apple\.com/bill", new[] { "internet", "computer" }),
            new ItemTagger(@"scaddabush", new[] { "food", "meal" }),
            new ItemTagger(@"bagel", new[] { "groceries" }),
            new ItemTagger(@"pharmac", new[] { "health", "drugs" }),
            new ItemTagger(@"the medicine shoppe", new[] { "health", "drugs" }),
            new ItemTagger(@"ida", new[] { "health", "drugs" }),
            new ItemTagger(@"swatch", new[] { "toys", "watch", "children" }),
            new ItemTagger(@"apple", new[] { "internet", "telephone" }),
            new ItemTagger(@"walmart", new[] { "groceries", "food" }),
            new ItemTagger(@"wal-mart", new[] { "groceries", "food" }),
            new ItemTagger(new [] { @"harbour front foods", @"chris' cheese mongers" }, new[] { "groceries", "food" }),
            new ItemTagger(new [] { @"popeyes", @"carnicero's" }, new[] { "meal", "food" }),
            new ItemTagger(new [] { @"petrocan", "north york general hos toronto on", "princess auto", @"air-serv" }, new[] { "transportation" }),
            new ItemTagger(new [] { @"paypal \*nintendo" }, new[] { "games" }),
            new ItemTagger(new [] { @"wahlburgers", "royal burger", "living well", "cocina mexicana barrie on", @"patchmon's tha", @"tasty chinese food" }, new[] { "meal", "food" }),
            new ItemTagger(new [] { @"boulangerie amour du p", "sumo ramen dorval", "living well" }, new[] { "meal", "food", "trip", "montreal" }),
            new ItemTagger(new [] { @"via rail"}, new[] { "trip", "montreal", "transportation" }),
            new ItemTagger(new [] { @"hotelscom" }, new[] { "trip", "montreal" }),
            new ItemTagger(@"nancy's cheese", new[] { "meal", "food", "cheese" }),
            new ItemTagger(@"staples", new[] { "alexander", "benjamin", "school-supplies" }),
            new ItemTagger(@"pizza", new[] { "food", "meal" }),
            new ItemTagger(@"deja vu", new[] { "food", "meal" }),
            new ItemTagger(@"king slice", new[] { "food", "meal" }),
            new ItemTagger(@"jerk king", new[] { "food", "meal" }),
            new ItemTagger(@"the yummy grill", new[] { "food", "meal" }),
            new ItemTagger(@"swadish grill", new[] { "food", "meal" }),
            new ItemTagger(new [] { @"oshkosh", "once upon a child" }, new[] { "clothing", "alexander", "benjamin", "constantin" }),
            new ItemTagger(@"stm", new[] { "transportation", "public-transportation" }),
            new ItemTagger(@"st\. michael's hospita", new[] { "charity" }),
            new ItemTagger(@"uber bv", new[] { "tip", "transportation" }),
            new ItemTagger(@"dollarama", new[] { "toilletries" }),
            new ItemTagger(@"blinds to go", new[] { "home-improvement" }),
            new ItemTagger(@"cdn tire store", new[] { "home-improvement" }),
            new ItemTagger(@"canadiantire", new[] { "home-improvement" }),
            new ItemTagger(@"dairy queen", new[] { "food", "dessert" }),
            new ItemTagger(@"baskin robbins", new[] { "food", "dessert" }),
            new ItemTagger(@"indigo", new[] { "books" }),
            new ItemTagger(@"economist", new[] { "newspapepr" }),
            new ItemTagger(@"shoppers drug mart", new[] { "pharmacy" }),
            new ItemTagger(@"shoppersdrugmart", new[] { "pharmacy" }),
            new ItemTagger(@"pharmasave the medical", new[] { "pharmacy" }),
            new ItemTagger(@"pusateris", new[] { "food", "groceries" }),
            new ItemTagger(@"coppa s fresh market", new[] { "food", "groceries" }),
            new ItemTagger(@"coextro", new[] { "internet" }),
            new ItemTagger(@"godaddy", new[] { "internet" }),
            new ItemTagger(@"\[IB\]", new[] { "internal" }),
            new ItemTagger(@"lcbo", new[] { "alocohol" }),
            new ItemTagger(@"amazon", new[] { "home-improvement" }),
            new ItemTagger(@"st. louis bar and gril", new[] { "food", "meal" }),
            new ItemTagger(@"a & w", new[] { "food", "meal" }),
            new ItemTagger(@"a&w", new[] { "food", "meal" }),
            new ItemTagger(@"the tickled toad", new[] { "food", "meal" }),
            new ItemTagger(@"amaya express", new[] { "food", "meal" }),
            new ItemTagger(@"netflix", new[] { "television" }),
            new ItemTagger(@"forest hill optical", new[] { "glasses" }),
            new ItemTagger(@"kit kat", new[] { "food", "dessert" }),
            new ItemTagger(@"^PP\*\d\d\d\dCODE", new[] { "internal" }),
            new ItemTagger(new [] { @"stincor", @"ones better living" }, new[] { "home-improvement" }),
            new ItemTagger(@"lids", new[] { "clothing" }),
            new ItemTagger(new [] { @"spin", @"li ning", @"merchantoftennis" }, new[] { "clothing" }),
            new ItemTagger(new [] { @"mgcs so birth", @"\[pr\]meridian", @"shatter sheppard", @"centreville etobicoke", "salarmy north york", @"cards gifts" }, new[] { "investigate" }),
            new ItemTagger(new [] { @"bflo museum of sci", @"the treehouse inc buffalo", @"albrightknox buffalo", @"frontier liquor buffalo", @"dash's market tonawanda", @"innbuffalo", @"royal motel \& campgrou" }, new[] { "trip", "buffalo" }),
            new ItemTagger(@"chudleigh's", new[] { "trip", "apple-picking" }),
            new ItemTagger(@"\[ck\]", new[] { "cheque" }),
            new ItemTagger(@"\[ds\]", new[] { "service-chargeable" }),
            new ItemTagger(@"\[dn\]", new[] { "not-service-chargeable" }),
            new ItemTagger(@"\[op\]", new[] { "online-purchase" }),
            new ItemTagger(@"pay to  cra", new[] { "taxes" }),
            new ItemTagger(@"economical ins\.", new[] { "insurance" }),
            new ItemTagger(@"canada\s*rit\/rif", new[] { "taxes" }),
            new ItemTagger(@"don ross", new[] { "painting", "home-improvement" }),
            new ItemTagger(@"digitalocean", new[] { "internet", "cloud-computing", "computers" }),
            new ItemTagger(@"change\.org", new[] { "charity" }),
            new ItemTagger(@"dynacare", new[] { "hospital", "health" }),
            new ItemTagger(@"soda-snack", new[] { "food", "snack" }),
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

            try
            {
                var creditCardStatements = GetCreditCardStatements();
                foreach (var statement in creditCardStatements)
                {
                    foreach (var file in statement.CsvFiles)
                    {
                        new CreditCardStatementFile(statement.AccountName, file).Import();
                    }
                }
            } catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            try
            {
                var bankStatements = GetBankStatements();
                foreach (var statement in bankStatements)
                {
                    foreach (var file in statement.CsvFiles)
                    {
                        new BankStatementFile(statement.AccountName, file).Import();
                    }
                }
            } catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine("Processing Items");
            var newItems = Item.GetAllNewItems();
            var i = 0;
            foreach (var item in newItems)
            {
                i += 1;

                Console.WriteLine($"Processing Item ({i} of {newItems.Length}):");
                Console.WriteLine(item);

                var itemsWithSameDescription = Item.FindByDescription(item.Description);
                var similarTags = itemsWithSameDescription
                    .SelectMany(item => item.Tags ?? Enumerable.Empty<Tag>())
                    .ToArray();

                if (similarTags.Length > 0 && IsAutoAssign)
                {
                    item.AddTags(similarTags);
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
                        var response = Console.ReadLine() ?? string.Empty;

                        repeat = false;
                        switch (response.ToLower().Trim())
                        {
                            case "":
                            case "y":
                            case "yes":
                                item.AddTags(similarTags);
                                break;
                            case "n":
                            case "no":
                                Console.WriteLine("Input tags:");
                                var commaSeparatedTagNames = ReadNewTags();
                                Console.WriteLine();

                                item.AddTags(commaSeparatedTagNames);
                                break;
                            default:
                                repeat = true;
                                break;
                        }
                    } while (repeat);
                }
                else if (this.IsAutoAssign)
                {
                    foreach (var tagging in this.Taggings.Where(t => t.IsMatch(item.Description)))
                    {
                        tagging.AddTags(item.Id);
                    }
                }
                else
                {
                    Console.WriteLine("Input tags:");
                    var commaSeparatedTagNames = ReadNewTags();
                    Console.WriteLine();

                    item.AddTags(commaSeparatedTagNames);
                }
            }
        }

        public static string GetCreditCardStatementsPath(IReadOnlyList<string> args)
        {
            return args[0];
        }

        public IEnumerable<AccountStatements> GetBankStatements()
        {
            Console.WriteLine("Getting Bank Statements");

            if (!BankStatementsPath.HasValue)
            {
                throw new Exception("Cannot find credit card statements");
            }

            return AccountStatements.GetAccountStatementsList(BankStatementsPath.Value);
        }

        public IEnumerable<AccountStatements> GetCreditCardStatements()
        {
            Console.WriteLine("Getting Credit Card Statements");

            if (!CreditCardStatementsPath.HasValue)
            {
                throw new Exception("Cannot find credit card statements");
            }

            return AccountStatements.GetAccountStatementsList(CreditCardStatementsPath.Value);
        }

        public static string ReadNewTags()
        {
            return Console.ReadLine() ?? string.Empty;
        }
    }
}

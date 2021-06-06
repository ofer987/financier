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
            new Tagging(@"sobeys", new[] { "food", "groceries" }),
            new Tagging(@"freshco", new[] { "food", "groceries" }),
            new Tagging(@"nofrills", new[] { "food", "groceries" }),
            new Tagging(@"metro", new[] { "food", "groceries" }),
            new Tagging(@"sababa fine foods", new[] { "food", "groceries" }),
            new Tagging(@"farm boy", new[] { "food", "groceries" }),
            new Tagging(@"iga express", new[] { "food", "groceries" }),
            new Tagging(@"instacart", new[] { "food", "groceries" }),
            new Tagging(@"airbnb", new[] { "lodging", "trip" }),
            new Tagging(@"ikea", new[] { "home-improvement" }),
            new Tagging(@"macaron and m", new[] { "coffee" }),
            new Tagging(@"zaza", new[] { "coffee" }),
            new Tagging(@"tommy cafe", new[] { "coffee" }),
            new Tagging(@"hotblack", new[] { "coffee" }),
            new Tagging(@"third wave coffee", new[] { "coffee" }),
            new Tagging(@"pilot coffee", new[] { "coffee" }),
            new Tagging(@"outpost coffee", new[] { "coffee" }),
            new Tagging(@"tim hortons", new[] { "coffee" }),
            new Tagging(new [] { @"tim hortons", @"strange love coffee", "jimmy's coffee" }, new[] { "coffee" }),
            new Tagging(@"goldstruck", new[] { "coffee" }),
            new Tagging(@"the social blend", new[] { "coffee" }),
            new Tagging(@"postal service", new[] { "mail" }),
            new Tagging(@"tdsb", new[] { "school" }),
            new Tagging(@"lego brand", new[] { "toys", "children" }),
            new Tagging(@"lego  yorkdale", new[] { "toys", "children" }),
            new Tagging(@"toys r us", new[] { "toys", "children" }),
            new Tagging(@"sephora", new[] { "makeup", "edith" }),
            new Tagging(@"citizenship \& imm", new[] { "edith", "citizenship" }),
            new Tagging(new [] { @"cash advance fee adj", "interest advances", "annual card fee waiver" }, new[] { "fees" }),
            new Tagging(new [] { @"loblaws", @"lucky moose food mart" }, new[] { "groceries", "food" }),
            new Tagging(@"marche le richmond", new[] { "bakery", "food", "coffee", "trip", "montreal" }),
            new Tagging(@"mike's fish market", new[] { "fish", "groceries", "food" }),
            new Tagging(new [] { @"st urbain", @"cosenza bakery" }, new[] { "bakery", "food" }),
            new Tagging(new [] { @"belle's bakery", "blackbird" }, new[] { "bakery", "food" }),
            new Tagging(@"the box donut", new[] { "bakery", "food", "dessert" }),
            new Tagging(@"gelato simply", new[] { "ice cream", "dessert" }),
            new Tagging(@"pamenar", new[] { "coffee" }),
            new Tagging(@"cafe myriade", new[] { "coffee", "trip", "montreal" }),
            new Tagging(@"applevale orchards", new[] { "trip", "family-activity" }),
            new Tagging(@"cafe des amis", new[] { "coffee", "trip", "montreal" }),
            new Tagging(@"cocoa latte", new[] { "coffee" }),
            new Tagging(@"cinnaholic", new[] { "coffee" }),
            new Tagging(@"cafe lali", new[] { "coffee", "trip", "montreal" }),
            new Tagging(@"uber *trip", new[] { "transportation", "taxi" }),
            new Tagging(@"ubr*", new[] { "food", "meal" }),
            new Tagging(new [] { @"szechuan express", @"kid lee", @"stock tc", @"grenadier cafe", @"flo's diner", @"mr\. greek mediter" }, new[] { "food", "meal" }),
            new Tagging(@"saffron spice kitchen", new[] { "food", "meal" }),
            new Tagging(@"hy's steakhouse*", new[] { "food", "meal" }),
            new Tagging(@"duke of york", new[] { "food", "meal" }),
            new Tagging(@"canada computers", new[] { "computer", "accessories" }),
            new Tagging(@"IQ Food", new[] { "coffee" }),
            new Tagging(@"shy coffee", new[] { "coffee" }),
            new Tagging(@"the library specia", new[] { "coffee" }),
            new Tagging(@"tim horton's", new[] { "coffee" }),
            new Tagging(@"famous player", new[] { "movies" }),
            new Tagging(new [] { @"neon", @"la vie en rose", @"dear born baby" }, new[] { "clothing" }),
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
            new Tagging(@"the medicine shoppe", new[] { "health", "drugs" }),
            new Tagging(@"ida", new[] { "health", "drugs" }),
            new Tagging(@"swatch", new[] { "toys", "watch", "children" }),
            new Tagging(@"apple", new[] { "internet", "telephone" }),
            new Tagging(@"walmart", new[] { "groceries", "food" }),
            new Tagging(@"wal-mart", new[] { "groceries", "food" }),
            new Tagging(new [] { @"harbour front foods", @"chris' cheese mongers" }, new[] { "groceries", "food" }),
            new Tagging(new [] { @"popeyes", @"carnicero's" }, new[] { "meal", "food" }),
            new Tagging(new [] { @"petrocan", "north york general hos toronto on", "princess auto", @"air-serv" }, new[] { "transportation" }),
            new Tagging(new [] { @"paypal \*nintendo" }, new[] { "games" }),
            new Tagging(new [] { @"wahlburgers", "royal burger", "living well", "cocina mexicana barrie on", @"patchmon's tha", @"tasty chinese food" }, new[] { "meal", "food" }),
            new Tagging(new [] { @"boulangerie amour du p", "sumo ramen dorval", "living well" }, new[] { "meal", "food", "trip", "montreal" }),
            new Tagging(new [] { @"via rail"}, new[] { "trip", "montreal", "transportation" }),
            new Tagging(new [] { @"hotelscom" }, new[] { "trip", "montreal" }),
            new Tagging(@"nancy's cheese", new[] { "meal", "food", "cheese" }),
            new Tagging(@"staples", new[] { "alexander", "benjamin", "school-supplies" }),
            new Tagging(@"pizza", new[] { "food", "meal" }),
            new Tagging(@"deja vu", new[] { "food", "meal" }),
            new Tagging(@"king slice", new[] { "food", "meal" }),
            new Tagging(@"jerk king", new[] { "food", "meal" }),
            new Tagging(@"the yummy grill", new[] { "food", "meal" }),
            new Tagging(@"swadish grill", new[] { "food", "meal" }),
            new Tagging(new [] { @"oshkosh", "once upon a child" }, new[] { "clothing", "alexander", "benjamin", "constantin" }),
            new Tagging(@"stm", new[] { "transportation", "public-transportation" }),
            new Tagging(@"st\. michael's hospita", new[] { "charity" }),
            new Tagging(@"uber bv", new[] { "tip", "transportation" }),
            new Tagging(@"dollarama", new[] { "toilletries" }),
            new Tagging(@"blinds to go", new[] { "home-improvement" }),
            new Tagging(@"cdn tire store", new[] { "home-improvement" }),
            new Tagging(@"canadiantire", new[] { "home-improvement" }),
            new Tagging(@"dairy queen", new[] { "food", "dessert" }),
            new Tagging(@"baskin robbins", new[] { "food", "dessert" }),
            new Tagging(@"indigo", new[] { "books" }),
            new Tagging(@"economist", new[] { "newspapepr" }),
            new Tagging(@"shoppers drug mart", new[] { "pharmacy" }),
            new Tagging(@"shoppersdrugmart", new[] { "pharmacy" }),
            new Tagging(@"pharmasave the medical", new[] { "pharmacy" }),
            new Tagging(@"pusateris", new[] { "food", "groceries" }),
            new Tagging(@"coppa s fresh market", new[] { "food", "groceries" }),
            new Tagging(@"coextro", new[] { "internet" }),
            new Tagging(@"godaddy", new[] { "internet" }),
            new Tagging(@"\[IB\]", new[] { "internal" }),
            new Tagging(@"lcbo", new[] { "alocohol" }),
            new Tagging(@"amazon", new[] { "home-improvement" }),
            new Tagging(@"st. louis bar and gril", new[] { "food", "meal" }),
            new Tagging(@"a & w", new[] { "food", "meal" }),
            new Tagging(@"a&w", new[] { "food", "meal" }),
            new Tagging(@"the tickled toad", new[] { "food", "meal" }),
            new Tagging(@"amaya express", new[] { "food", "meal" }),
            new Tagging(@"netflix", new[] { "television" }),
            new Tagging(@"forest hill optical", new[] { "glasses" }),
            new Tagging(@"kit kat", new[] { "food", "dessert" }),
            new Tagging(@"^PP\*\d\d\d\dCODE", new[] { "internal" }),
            new Tagging(new [] { @"stincor", @"ones better living" }, new[] { "home-improvement" }),
            new Tagging(@"lids", new[] { "clothing" }),
            new Tagging(new [] { @"spin", @"li ning", @"merchantoftennis" }, new[] { "clothing" }),
            new Tagging(new [] { @"mgcs so birth", @"\[pr\]meridian", @"shatter sheppard", @"centreville etobicoke", "salarmy north york", @"cards gifts" }, new[] { "investigate" }),
            new Tagging(new [] { @"bflo museum of sci", @"the treehouse inc buffalo", @"albrightknox buffalo", @"frontier liquor buffalo", @"dash's market tonawanda", @"innbuffalo", @"royal motel \& campgrou" }, new[] { "trip", "buffalo" }),
            new Tagging(@"chudleigh's", new[] { "trip", "apple-picking" }),
            new Tagging(@"\[ck\]", new[] { "cheque" }),
            new Tagging(@"\[ds\]", new[] { "service-chargeable" }),
            new Tagging(@"\[dn\]", new[] { "not-service-chargeable" }),
            new Tagging(@"\[op\]", new[] { "online-purchase" }),
            new Tagging(@"pay to  cra", new[] { "taxes" }),
            new Tagging(@"economical ins\.", new[] { "insurance" }),
            new Tagging(@"canada\s*rit\/rif", new[] { "taxes" }),
            new Tagging(@"don ross", new[] { "painting", "home-improvement" }),
            new Tagging(@"digitalocean", new[] { "internet", "cloud-computing", "computers" }),
            new Tagging(@"change\.org", new[] { "charity" }),
            new Tagging(@"dynacare", new[] { "hospital", "health" }),
            new Tagging(@"soda-snack", new[] { "food", "snack" }),
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

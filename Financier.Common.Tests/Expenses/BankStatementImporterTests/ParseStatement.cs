using System;
using System.Linq;
using System.Collections;
using System.IO;
using NUnit.Framework;

using Financier.Common.Expenses;

namespace Financier.Common.Tests.Expenses.BankStatementImporterTests
{
    public class ParseStatement : DatabaseAbstractFixture
    {
        public static IEnumerable ValidTestCases
        {
            get
            {
                {
                    const string input = 
@"Account,First Bank Card,Transaction Type,Date Posted,Transaction Amount,Description
'04183988880','5007660790617248',DEBIT,20190104,-4.25,[PR]SAM JAMES COFFE
'04183988880','5007660790617248',DEBIT,20190107,-14.0,[PR]ALMINZ KAKANIN
'04183988880','5007660790617248',CREDIT,20190108,425,[WI]Money From EMPLOYER";
                    var output = new[]
                    {
                        new BankStatementRecord
                        {
                            AccountNumber = "'04183988880'",
                            FirstBankCardNumber = "'5007660790617248'",
                            TransactionTypeString = "DEBIT",
                            PostedAt = "20190104",
                            Amount = "-4.25",
                            Description = "[PR]SAM JAMES COFFE"
                        },
                        new BankStatementRecord
                        {
                            AccountNumber = "'04183988880'",
                            FirstBankCardNumber = "'5007660790617248'",
                            TransactionTypeString = "DEBIT",
                            PostedAt = "20190107",
                            Amount = "-14.0",
                            Description = "[PR]ALMINZ KAKANIN"
                        },
                        new BankStatementRecord
                        {
                            AccountNumber = "'04183988880'",
                            FirstBankCardNumber = "'5007660790617248'",
                            TransactionTypeString = "CREDIT",
                            PostedAt = "20190108",
                            Amount = "425",
                            Description = "[WI]Money From EMPLOYER"
                        }
                    };

                    yield return new TestCaseData(input).Returns(output);
                }
            }
        }

        protected override void InitDb()
        {
        }

        [Test]
        [TestCaseSource(nameof(ValidTestCases))]
        public BankStatementRecord[] Test_Expenses_BankStatementImporter_ParseStatement_ValidRecords(string input)
        {
            return new BankStatementImporter().ParseStatement(GetStream(input));
        }

        public static IEnumerable InvalidTestCases
        {
            get
            {
                {
                    const string input = 
@"Some data retrieved from my bank
Account,First Bank Card,Transaction Type,Date Posted,Transaction Amount,Description
'04183988880','5007660790617248',DEBIT,20190104,-4.25,[PR]SAM JAMES COFFE";

                    yield return new TestCaseData(input);
                }

                {
                    const string input = 
@"Account, First Bank Card,Transaction Type ,Date Posted,Transaction Amount,Description
'04183988880','5007660790617248',DEBIT,20190104,-4.25,[PR]SAM JAMES COFFE";

                    yield return new TestCaseData(input);
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(InvalidTestCases))]
        public void Test_Expenses_BankStatementImporter_ParseStatement_InvalidRecords(string input)
        {
            Assert.Throws<CsvHelper.HeaderValidationException>(() => new BankStatementImporter().ParseStatement(GetStream(input)));
        }

        private Stream GetStream(string str)
        {
            var buffer = str.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
            return new MemoryStream(buffer);
        }
    }
}

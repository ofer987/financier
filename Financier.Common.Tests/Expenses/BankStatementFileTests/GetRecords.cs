using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.BankStatementFileTests
{
    public class GetRecords : Tests
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
                            ItemId = "1",
                            Number = "'04183988880'",
                            FirstBankCardNumber = "'5007660790617248'",
                            TransactionTypeString = "DEBIT",
                            PostedAt = "20190104",
                            Amount = "-4.25",
                            Description = "[PR]SAM JAMES COFFE"
                        },
                        new BankStatementRecord
                        {
                            ItemId = "2",
                            Number = "'04183988880'",
                            FirstBankCardNumber = "'5007660790617248'",
                            TransactionTypeString = "DEBIT",
                            PostedAt = "20190107",
                            Amount = "-14.0",
                            Description = "[PR]ALMINZ KAKANIN"
                        },
                        new BankStatementRecord
                        {
                            ItemId = "3",
                            Number = "'04183988880'",
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

        [AllowNull]
        public string AccountName { get; private set; }

        protected override void InitStorage()
        {
            AccountName = "Mr Bean";
        }

        [Test]
        [TestCaseSource(nameof(ValidTestCases))]
        public BankStatementRecord[] Test_Expenses_BankStatementFile_ParseStatement_ValidRecords(string input)
        {
            return new BankStatementFile(AccountName, GetStream(input), new DateTime(2019, 1, 1)).GetRecords();
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
        public void Test_Expenses_BankStatementFile_ParseStatement_InvalidRecords(string input)
        {
            Assert.Throws<CsvHelper.HeaderValidationException>(() => new BankStatementFile(AccountName, GetStream(input), new DateTime(2019, 1, 1)).GetRecords());
        }

        private Stream GetStream(string str)
        {
            var buffer = str.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
            return new MemoryStream(buffer);
        }
    }
}

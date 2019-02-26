using CsvHelper.Configuration.Attributes;

namespace Financier.Common.Expenses.Models
{
    // TODO: Trim values and set Valid function
    public class BankStatementRecord : IStatementRecord
    {
        public enum TransactionTypes { Debit, Credit }

        [Name("Account")]
        public string Number { get; set; }

        [Name("First Bank Card")]
        public string FirstBankCardNumber { get; set; }

        [Name("Transaction Type")]
        public string TransactionTypeString { get; set; }

        [Name("Date Posted")]
        public string PostedAt { get; set; }

        [Name("Transaction Amount")]
        public string Amount { get; set; }

        [Name("Description")]
        public string Description { get; set; }

        public TransactionTypes TransactionType
        {
            get
            {
                switch ((TransactionTypeString ?? string.Empty).Trim().ToUpper())
                {
                    case "CREDIT":
                        return TransactionTypes.Credit;
                    case "DEBIT":
                    default:
                        return TransactionTypes.Debit;
                }
            }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode() 
                + FirstBankCardNumber.GetHashCode()
                + TransactionTypeString.GetHashCode()
                + PostedAt.GetHashCode()
                + Amount.GetHashCode()
                + Description.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as BankStatementRecord;
            if (other == null)
            {
                return false;
            }

            if (Number != other.Number
                || FirstBankCardNumber != other.FirstBankCardNumber
                || TransactionTypeString != other.TransactionTypeString
                || PostedAt != other.PostedAt
                || Amount != other.Amount
                || Description != other.Description)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{nameof(Number)}: ({Number})\n{nameof(FirstBankCardNumber)}: ({FirstBankCardNumber})\n{nameof(TransactionType)}: ({TransactionType})\n{nameof(PostedAt)}: ({PostedAt})\n{nameof(Amount)}: ({Amount})\n{nameof(Description)}: ({Description})";
        }
    }
}

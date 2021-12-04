// TODO: Remove?
// enum ExpenseTypes { Credit, Debit };

class Listing {
  public creditAmount: number;
  public debitAmount: number;

  public get profitAmount(): number {
    return this.creditAmount - this.debitAmount;
  }

  public constructor(creditAmount: number, debitAmount: number) {
    this.creditAmount = creditAmount;
    this.debitAmount = debitAmount;
  }
}

export { Listing };

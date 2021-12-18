// TODO: Remove?
// enum ExpenseTypes { Credit, Debit };

class Amount {
  public credit: number;
  public debit: number;

  public get profit(): number {
    return this.credit - this.debit;
  }

  public constructor(credit: number, debit: number) {
    this.credit = credit;
    this.debit = debit;
  }
}

export { Amount };

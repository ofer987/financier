import Nullable from "./Nullable";

enum ExpenseTypes { Credit,  Debit };

interface Listing extends Nullable  {
  startAt: Date;
  endAt: Date;
  tags: string[];
  amount: number;
  expenseType: ExpenseTypes

  toString(): string;
}

export { Listing, ExpenseTypes };

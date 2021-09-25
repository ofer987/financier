import Nullable from "./Nullable";

enum ExpenseTypes { Credit,  Debit };

interface Listing extends Nullable  {
  tags: string[];
  amount: number;
  expenseType: ExpenseTypes
}

export { Listing, ExpenseTypes };

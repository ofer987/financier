import Nullable from "./Nullable";

// TODO: Remove?
enum ExpenseTypes { Credit, Debit };

interface Listing extends Nullable  {
  tags: string[];
  creditAmount: number;
  debitAmount: number;

  toString(): string;
}

export { Listing, ExpenseTypes };

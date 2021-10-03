import _ from "underscore";
import { Listing, ExpenseTypes } from "./Listing";

class DetailedListing implements Listing {
  public startAt: Date;
  public endAt: Date;
  public tags: string[];
  public amount: number;
  public expenseType: ExpenseTypes;

  get isNull(): boolean {
    return false;
  }

  constructor(year: number, month: number, amount: number, expenseType: ExpenseTypes) {
    this.startAt = new Date(year, month);
    this.endAt = new Date(year, month);
    this.amount = amount;
    this.expenseType = expenseType;
    this.tags = [];
  }

  toString(): string {
    return this.startAt.toString();
  }
}

export default DetailedListing;

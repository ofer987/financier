import _ from "underscore";
import { Listing } from "./Listing";

class MonthlyListing implements Listing {
  public year: number;
  public month: number;
  public tags: string[];
  public creditAmount: number;
  public debitAmount: number;

  get isNull(): boolean {
    return false;
  }

  constructor(year: number, month: number, creditAmount: number, debitAmount: number) {
    this.year = year;
    this.month = month;
    this.creditAmount = creditAmount;
    this.debitAmount = debitAmount;
    this.tags = [];
  }

  toString(): string {
    return `${this.year.toString()} + ${this.month.toString()}`;
  }
}

export default MonthlyListing;

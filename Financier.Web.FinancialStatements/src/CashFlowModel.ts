import _ from "underscore";
import lodash from "lodash";

import { Listing, ExpenseTypes } from "./Listing";

class CashFlowModel implements Listing {
  public startAt: Date;
  public endAt: Date;
  public tags: string[];
  public amount: number;
  public expenseType: ExpenseTypes;

  static hasTag(tag: string, listing: Listing): boolean {
    if (_.contains(listing.tags, tag)) {
      return true;
    }

    return false;
  }

  get isNull(): boolean {
    return false;
  }

  constructor(startAt: Date, endAt: Date, tags: { name: string }[], amount: number, expenseType: ExpenseTypes) {
    this.startAt = startAt;
    this.endAt = endAt;
    this.amount = amount;
    this.expenseType = expenseType;

    this.init(tags);
  }

  init(values: { name: string }[]): void {
    this.tags = values
      .map(values => values.name)
      .map(name => lodash.startCase(name));
  }

  toString(): string {
    return this.tags.join("-");
  }
}

export default CashFlowModel;

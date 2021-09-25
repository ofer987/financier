import { Listing, ExpenseTypes } from "./Listing";

class CashFlowModel implements Listing {
  public startAt: Date;
  public endAt: Date;
  public tags: string[];
  public amount: number;
  public expenseType: ExpenseTypes;

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
    this.tags = values.map(values => values.name);
  }
}

export default CashFlowModel;

import Listing from "./Listing";

class CashTagsModel implements Listing {
  startAt: Date;
  endAt: Date;
  tags: string[];
  amount: number;

  get isNull(): boolean {
    return false;
  }

  constructor(startAt: Date, endAt: Date, tags: { name: string }[], amount: number) {
    this.startAt = startAt;
    this.endAt = endAt;
    this.amount = amount;

    this.init(tags);
  }

  init(values: { name: string }[]): void {
    this.tags = values.map(values => values.name);
  }
}

export default CashTagsModel;

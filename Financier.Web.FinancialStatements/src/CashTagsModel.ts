import Listing from "./CashFlowModel/Listing";

class CashTagsModel implements Listing {
  startAt: Date;
  endAt: Date;
  tags: { name: string }[];
  amount: number;

  constructor(startAt: Date, endAt: Date, tags: { name: string }[], amount: number) {
    this.startAt = startAt;
    this.endAt = endAt;
    this.tags = tags;
    this.amount = amount;
  }

  // name(): string {
  //   return this.tags
  //     .map(tag => tag.name)
  //     .join(", ");
  // }
  //
  // at(): string {
  //   return `${this.startAt.getFullYear()}-${this.endAt.getMonth()}`;
  // }
}

export default CashTagsModel;

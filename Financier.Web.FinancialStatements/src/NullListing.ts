import Listing from "./CashFlowModel/Listing";

class NullListing implements Listing {
  tags: string[];
  amount: number;

  get isNull(): boolean {
    return true;
  }

  constructor() {
    this.tags = [];
    this.amount = 0;
  }
}

export default NullListing;

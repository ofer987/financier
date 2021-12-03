import { Listing } from "./Listing";

class NullListing implements Listing {
  public tags = []
  public creditAmount = 0;
  public debitAmount = 0;

  public get profitAmount(): number {
    return this.creditAmount - this.debitAmount;
  }

  public get isNull(): boolean {
    return true;
  }

  public toString(): string {
    return Math.random().toString(16).substr(2, 13);
  }
}

export default NullListing;

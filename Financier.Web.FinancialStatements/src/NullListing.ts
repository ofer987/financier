import { Listing } from "./Listing";

class NullListing implements Listing {
  public startAt = new Date();
  public endAt = new Date();
  public tags = []
  public amount = 0;
  public expenseType = undefined;

  get isNull(): boolean {
    return true;
  }

  toString(): string {
    return Math.random().toString(16).substr(2, 13);
  }
}

export default NullListing;

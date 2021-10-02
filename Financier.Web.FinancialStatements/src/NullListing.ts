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
}

export default NullListing;

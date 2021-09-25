import { Listing } from "./Listing";

class NullListing implements Listing {
  public tags = []
  public amount = 0;
  public expenseType = undefined;

  get isNull(): boolean {
    return true;
  }
}

export default NullListing;

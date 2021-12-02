import _ from "underscore";
import lodash from "lodash";

import { Listing } from "./Listing";

class DetailedListing implements Listing {
  public tags: string[];
  public creditAmount: number;
  public debitAmount: number;

  static hasTag(tag: string, listing: Listing): boolean {
    if (_.contains(listing.tags, tag)) {
      return true;
    }

    return false;
  }

  get isNull(): boolean {
    return false;
  }

  constructor(tags: { name: string }[], creditAmount: number, debitAmount) {
    this.creditAmount = creditAmount;
    this.debitAmount = debitAmount;

    this.init(tags);
  }

  public toString(): string {
    return this.tags.join("-");
  }

  private init(values: { name: string }[]): void {
    this.tags = values
      .map(values => values.name)
      .map(name => lodash.startCase(name));
  }
}

export default DetailedListing;

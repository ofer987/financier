import * as React from "react";
import _ from "underscore";

import Value from "./Value";
import { Listing } from "./Listing";
import NullListing from "./NullListing";
import FilterableController from "./FilterableController";

interface Props {
  debits: Listing[];
  credits: Listing[];
  enabledTags: string[];
}

class Values extends FilterableController {
  decimalCount = 2;

  get accountingFormattedProfit(): string {
    const profit = (this.totalCredits - this.totalDebits);

    if (profit >= 0) {
      return profit.toFixed(this.decimalCount);
    }

    return `(${(0 - profit).toFixed(this.decimalCount)})`;
  }

  render() {
    return (
      <div className="values">
        <h2>Items</h2>
        <div className="header">
          <div className="name">Name</div>
          <div className="credits">Credits</div>
          <div className="debits">Debits</div>
          <div className="profit">Profit (Deficit)</div>
        </div>
        <div className="items">
          {this.enabledCredits.map(listing => <Value debit={new NullListing()} credit={listing} key={listing.toString()} />)}
          {this.enabledDebits.map(listing => <Value debit={listing} credit={new NullListing()} key={listing.toString()} />)}
        </div>
        <div className="total">
          <div className="name">Total</div>
          <div className="credits">{this.totalCredits.toFixed(this.decimalCount)}</div>
          <div className="debits">{this.totalDebits.toFixed(this.decimalCount)}</div>
          <div className="profit">{this.accountingFormattedProfit}</div>
        </div>
      </div>
    );
  }
}

export default Values;

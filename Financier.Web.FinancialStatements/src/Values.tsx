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
  render() {
    return (
      <div className="values">
        <h2>Items</h2>
        <div className="header">
          <div className="name">Name</div>
          <div className="credits">Credits</div>
          <div className="debits">Debits</div>
        </div>
        <div className="items">
          {this.enabledCredits.map(listing => <Value debit={new NullListing()} credit={listing} />)}
          {this.enabledDebits.map(listing => <Value debit={listing} credit={new NullListing()} />)}
        </div>
        <div className="total">
          <div className="name">Total</div>
          <div className="credits">{this.totalCredits.toFixed(2)}</div>
          <div className="debits">{this.totalDebits.toFixed(2)}</div>
        </div>
      </div>
    );
  }
}

export default Values;

import * as React from "react";
import _ from "underscore";

import DetailedValue from "./DetailedValue";
import { Listing } from "./Listing";
import NullListing from "./NullListing";
import { DetailedListing } from "./DetailedCashFlow";

class DetailedValues extends React.Component<DetailedListing[]> {
  decimalCount = 2;

  public get totalCredits(): number {
    var amounts = this.props
      .map(item => item.listing)
      .map(item => item.creditAmount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get totalDebits(): number {
    var amounts = this.props
      .map(item => item.listing)
      .map(item => item.debitAmount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get totalProfit(): number {
    var amounts = this.props
      .map(item => item.listing)
      .map(item => item.profitAmount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get accountingFormattedProfit(): string {
    const profit = this.totalProfit;
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
          {this.props.map(item => <DetailedValue {...item} key={item.toString()} />)}
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

export default DetailedValues;

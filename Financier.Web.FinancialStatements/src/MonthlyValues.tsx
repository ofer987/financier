import * as React from "react";
import _ from "underscore";

import MonthlyValue from "./MonthlyValue";
import { Listing } from "./Listing";
import NullListing from "./NullListing";
import FilterableController from "./FilterableController";
import { MonthlyProps, MonthlyProp } from "./MonthlyGraph";

class MonthlyValues extends React.Component<MonthlyProps> {
  decimalCount = 2;

  get totalCredits(): number {
    var amounts = this.props.dates.map(item => item.credit.amount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  get totalDebits(): number {
    var amounts = this.props.dates.map(item => item.debit.amount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

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
          <div className="name">When</div>
          <div className="credits">Credits</div>
          <div className="debits">Debits</div>
          <div className="profit">Profit (Deficit)</div>
        </div>
        <div className="items">
          { /* TODO: Display credits and debits should be children of dates (ats) */ }
          {this.props.dates.map(item => <MonthlyValue at={item.at} credit={item.credit} debit={item.debit} />)}
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

export default MonthlyValues;

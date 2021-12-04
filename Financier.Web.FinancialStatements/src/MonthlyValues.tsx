import * as React from "react";
import _ from "underscore";

import MonthlyValue from "./MonthlyValue";
import { Listing } from "./Listing";
import NullListing from "./NullListing";
import FilterableController from "./FilterableController";
import { MonthlyProps, MonthlyProp } from "./MonthlyGraph";

interface Prop extends MonthlyProps {
  dateRange: [Date, Date];
}

class MonthlyValues extends React.Component<Prop> {
  decimalCount = 2;

  get validMonths(): MonthlyProp[] {
    return this.props.dates
      .filter(date => this.isMonthValid(date.at));
  }

  get totalCredits(): number {
    var amounts = this.validMonths.map(item => item.credit.amount);

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

  public render() {
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
          {this.dates}
          {this.validMonths
            .map(item => <MonthlyValue at={item.at} credit={item.credit} debit={item.debit} key={item.at.toString()} dateRange={this.props.dateRange} />)}
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

  private dates(): { year: number, month: number }[] {
    const startAt = this.props.dateRange[0];
    const endAt = this.props.dateRange[1];

    let results: { year: number, month: number }[] = [];
    for (let at = startAt; at <= endAt; at = new Date(at.setMonth(at.getMonth() + 1))) {
      results.push({
        year: at.getFullYear(),
        month: at.getMonth() + 1
      });
    }

    return results;
  }

  private isMonthValid(at: Date): boolean {
    const foundProp = this.props.dates
      .find(date => {
        return true
          && date.year == at.getFullYear()
          && date.month == at.getMonth()
      });

    if (typeof (foundProp) == "undefined") {
      return false;
    }

    if (foundProp.listing.isNull) {
      return false;
    }

    return true;
  }
}

export default MonthlyValues;

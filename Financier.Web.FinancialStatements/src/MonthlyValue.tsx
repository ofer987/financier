import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";
import { MonthlyListing } from "./MonthlyCashFlow";

import { Listing } from "./Listing";

// interface Props extends MonthlyProp {
//   dateRange?: [Date, Date];
// }

class MonthlyValue extends React.Component<MonthlyListing> {
  private decimalCount = 2;

  public get name(): string {
    const at = new Date(this.year, this.month);
    const year = this.year;
    const monthName = d3TimeFormat.timeFormat("%B")(at);

    return `${year} - ${monthName}`;
  }

  public get year(): number {
    return this.props.year;
  }

  public get month(): number {
    return this.props.month + 1;
  }

  public get creditAmount(): number {
    return this.props.listing.creditAmount;
  }

  public get debitAmount(): number {
    return this.props.listing.debitAmount;
  }

  public get accountingFormattedProfit(): string {
    const profit = (this.creditAmount - this.debitAmount);

    if (profit >= 0) {
      return profit.toFixed(this.decimalCount);
    }

    return `(${(0 - profit).toFixed(this.decimalCount)})`;
  }

  public render() {
    return (
      <div className="item" id={this.name} key={this.name} onClick={() => this.navigateToDetailedView(this.year, this.month)}>
        <div className="name">
          {this.name}
        </div>
        <div className="credit">
          {this.creditAmount.toFixed(this.decimalCount)}
        </div>
        <div className="debit">
          {this.debitAmount.toFixed(this.decimalCount)}
        </div>
        <div className="profit">
          {this.accountingFormattedProfit}
        </div>
      </div>
    )
  }

  private navigateToDetailedView(year: number, month: number) {
    window.location.pathname = `/detailed-view/year/${this.year}/month/${this.month}`;
  }
}

export default MonthlyValue;

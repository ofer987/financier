import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";

import { MonthlyRecord } from "./MonthlyRecord";

// interface Props extends MonthlyProp {
//   dateRange?: [Date, Date];
// }

interface Props {
  record: MonthlyRecord;
}

class MonthlyValue extends React.Component<Props> {
  private decimalCount = 2;

  public get name(): string {
    const at = new Date(this.year, this.month);
    const year = this.year;
    const monthName = d3TimeFormat.timeFormat("%B")(at);

    return `${year} - ${monthName}`;
  }

  public get isPrediction(): boolean {
    return this.props.record.isPrediction;
  }

  public get year(): number {
    return this.props.record.year;
  }

  public get month(): number {
    return this.props.record.month;
  }

  public get credit(): number {
    return this.props.record.amount.credit;
  }

  public get debit(): number {
    return this.props.record.amount.debit;
  }

  public get accountingFormattedProfit(): string {
    const profit = (this.credit - this.debit);

    if (profit >= 0) {
      return profit.toFixed(this.decimalCount);
    }

    return `(${(0 - profit).toFixed(this.decimalCount)})`;
  }

  public render() {
    let month = this.month + 1;

    if (!this.isPrediction) {
      return (
        <div className="item clickable" id={this.name} key={this.name} onClick={() => this.navigateToDetailedView(this.year, month)}>
          <div className="name">
            {this.name}
          </div>
          <div className="credit">
            {this.credit.toFixed(this.decimalCount)}
          </div>
          <div className="debit">
            {this.debit.toFixed(this.decimalCount)}
          </div>
          <div className="profit">
            {this.accountingFormattedProfit}
          </div>
        </div>
      );
    } else {
      return (
        <div className="item not-clickable" id={this.name} key={this.name}>
          <div className="name">
            {this.name}
          </div>
          <div className="credit">
            {this.credit.toFixed(this.decimalCount)}
          </div>
          <div className="debit">
            {this.debit.toFixed(this.decimalCount)}
          </div>
          <div className="profit">
            {this.accountingFormattedProfit}
          </div>
        </div>
      );
    }
  }

  private navigateToDetailedView(year: number, month: number): void {
    window.location.pathname = `/detailed-view/year/${year}/month/${month}`;
  }
}

export default MonthlyValue;

import * as React from "react";
import _  from "underscore";
import * as d3TimeFormat from "d3-time-format";

import { MonthlyRecord } from "./MonthlyRecord";

import "./MonthlyValue.scss";

// interface Props extends MonthlyProp {
//   dateRange?: [Date, Date];
// }

interface Props {
  record: MonthlyRecord;
}

class MonthlyValue extends React.Component<Props> {
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

  public get credit(): string {
    return this.formatted(this.props.record.amount.credit);
  }

  public get debit(): string {
    return this.formatted(this.props.record.amount.debit);
  }

  public render() {
    let month = this.month + 1;

    if (!this.isPrediction) {
      return (
        <div className="MonthlyValue">
          <div className="item clickable" id={this.name} key={this.name} onClick={() => this.navigateToDetailedView(this.year, month)}>
            {this.renderChildren()}
          </div>
        </div>
      );
    } else {
      return (
        <div className="MonthlyValue">
          <div className="item not-clickable" id={this.name} key={this.name}>
            {this.renderChildren()}
          </div>
        </div>
      );
    }
  }

  private renderChildren() {
    return (
      <>
        <div className="name">
          {this.name}
        </div>
        <div className="credit number">
          {this.credit}
        </div>
        <div className="debit number">
          {this.debit}
        </div>
        <div className="profit number">
          {this.accountingFormattedProfit}
        </div>
      </>
    );
  }

  private get accountingFormattedProfit(): string {
    let profit = this.props.record.amount.profit;

    if (profit < 0) {
      profit = 0 - profit;
      return `(${this.formatted(profit)})`;
    }
    return this.formatted(profit);
  }

  private formatted(value: number): string {
    return value.toLocaleString("en-CA", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  private navigateToDetailedView(year: number, month: number): void {
    window.location.pathname = `/detailed-view/year/${year}/month/${month}`;
  }
}

export default MonthlyValue;

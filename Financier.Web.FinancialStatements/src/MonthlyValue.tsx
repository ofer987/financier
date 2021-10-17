import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";

import { Listing } from "./Listing";

interface Props {
  at: Date;
  debit: Listing;
  credit: Listing;
}

class MonthlyValue extends React.Component<Props> {
  private decimalCount = 2;

  get name(): string {
    const year = this.year;
    const month = d3TimeFormat.timeFormat("%B")(this.props.at);

    return `${year} - ${month}`;
  }

  get year(): number {
    return this.props.at.getFullYear();
  }

  get month(): number {
    return this.props.at.getMonth() + 1;
  }

  get creditAmount(): number {
    return this.props.credit.amount;
  }

  get debitAmount(): number {
    return this.props.debit.amount;
  }

  get accountingFormattedProfit(): string {
    const profit = (this.creditAmount - this.debitAmount);

    if (profit >= 0) {
      return profit.toFixed(this.decimalCount);
    }

    return `(${(0 - profit).toFixed(this.decimalCount)})`;
  }

  render() {
    return (
      <div className="item" id={this.name} key={this.name} onClick={() => this.toDetailedView(this.year, this.month)}>
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

  private toDetailedView(year: number, month: number) {
    window.location.pathname = `/detailed-view/year/${this.year}/month/${this.month}`;
  }
}

export default MonthlyValue;

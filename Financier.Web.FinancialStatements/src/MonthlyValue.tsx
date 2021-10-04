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

  render() {
    return (
      <div className="item" id={this.name} key={this.name}>
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

  get name(): string {
    const year = this.props.at.getFullYear();
    const month = d3TimeFormat.timeFormat("%B")(this.props.at);

    return `${year} - ${month}`;
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
}

export default MonthlyValue;

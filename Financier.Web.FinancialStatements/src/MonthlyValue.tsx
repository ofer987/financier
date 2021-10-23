import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";
import { MonthlyProp } from "./MonthlyGraph";

import { Listing } from "./Listing";

interface Props extends MonthlyProp {
  dateRange?: [Date, Date];
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
    if (typeof (this.props.dateRange) != "undefined") {
      console.log("has date range");
      const fromYear = this.props.dateRange[0].getFullYear();
      const fromMonth = this.props.dateRange[0].getMonth() + 1;

      const toYear = this.props.dateRange[1].getFullYear();
      const toMonth = this.props.dateRange[1].getMonth() + 1;

      window.location.search = `from-year=${fromYear}&from-month=${fromMonth}&to-year=${toYear}&to-month=${toMonth}`;
    }

    window.location.pathname = `/detailed-view/year/${this.year}/month/${this.month}`;
  }
}

export default MonthlyValue;

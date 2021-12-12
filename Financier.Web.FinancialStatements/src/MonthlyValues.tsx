import * as React from "react";
import _ from "underscore";

import MonthlyValue from "./MonthlyValue";
import { MonthlyRecord } from "./MonthlyRecord";

interface Props {
  records: MonthlyRecord[];
}

class MonthlyValues extends React.Component<Props> {
  private decimalCount = 2;

  public get records(): MonthlyRecord[] {
    return this.props.records;
  }

  public get totalCredits(): number {
    var amounts = this.records
      .map(item => item.amount)
      .map(item => item.credit);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get totalDebits(): number {
    var amounts = this.records
      .map(item => item.amount)
      .map(item => item.debit);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get accountingFormattedProfit(): string {
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
          { /*this.dates */ }
          {this.records
            .map(item => <MonthlyValue record={item} />)}
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

  // private dates(): { year: number, month: number }[] {
  //   const startYear = this.props[0].year;
  //   const endYear = this.props[this.props.length - 1].year;
  //
  //   let results: { year: number, month: number }[] = [];
  //   for (let at = startAt; at <= endAt; at = new Date(at.setMonth(at.getMonth() + 1))) {
  //     results.push({
  //       year: at.getFullYear(),
  //       month: at.getMonth() + 1
  //     });
  //   }
  //
  //   return results;
  // }

  // private isMonthValid(at: Date): boolean {
  //   const foundProp = this.props.dates
  //     .find(date => {
  //       return true
  //         && date.year == at.getFullYear()
  //         && date.month == at.getMonth()
  //     });
  //
  //   if (typeof (foundProp) == "undefined") {
  //     return false;
  //   }
  //
  //   if (foundProp.listing.isNull) {
  //     return false;
  //   }
  //
  //   return true;
  // }
}

export default MonthlyValues;

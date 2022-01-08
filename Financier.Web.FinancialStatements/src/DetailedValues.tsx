import * as React from "react";
import _ from "underscore";

import DetailedValue from "./DetailedValue";
import { DetailedRecord } from "./DetailedRecord";

interface Props {
  year: number;
  month: number;
  records: DetailedRecord[];
}

class DetailedValues extends React.Component<Props> {
  decimalCount = 2;

  public get year(): number {
    return this.props.year;
  }

  public get month(): number {
    return this.props.month;
  }

  public get totalCredits(): number {
    var amounts = this.props.records
      .map(item => item.amount)
      .map(item => item.credit);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get totalDebits(): number {
    var amounts = this.props.records
      .map(item => item.amount)
      .map(item => item.debit);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get totalProfit(): number {
    var amounts = this.props.records
      .map(item => item.amount)
      .map(item => item.profit);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get accountingFormattedProfit(): string {
    const profit = this.totalProfit;
    if (profit >= 0) {
      return profit.toFixed(this.decimalCount);
    }

    return `(${(0 - profit).toFixed(this.decimalCount)})`;
  }

  public toKey(value: DetailedRecord): string {
    return value.tags.join("-");
  }

  render() {
    return (
      <div className="values">
        <h2>Items</h2>
        <div className="header">
          <div className="name">Name</div>
          <div className="credits number">Credits</div>
          <div className="debits number">Debits</div>
          <div className="profit number">Profit (Deficit)</div>
        </div>
        <div className="items">
          {this.props.records.map(item => <DetailedValue record={item} key={this.toKey(item)} year={this.year} month={this.month} />)}
        </div>
        <div className="total">
          <div className="name">Total</div>
          <div className="credits number">{this.totalCredits.toFixed(this.decimalCount)}</div>
          <div className="debits number">{this.totalDebits.toFixed(this.decimalCount)}</div>
          <div className="profit number">{this.accountingFormattedProfit}</div>
        </div>
      </div>
    );
  }
}

export default DetailedValues;

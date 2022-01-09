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
  public get records(): DetailedRecord[] {
    return this.props.records;
  }

  public get year(): number {
    return this.props.year;
  }

  public get month(): number {
    return this.props.month;
  }

  public get totalCredits(): string {
    const amounts = this.props.records
      .map(item => item.amount)
      .map(item => item.credit);

    const total = _.reduce(amounts, (total, amount) => total + amount) || 0;
    return this.formatted(total);
  }

  public get totalDebits(): string {
    const amounts = this.props.records
      .map(item => item.amount)
      .map(item => item.debit);

    const total = _.reduce(amounts, (total, amount) => total + amount) || 0;
    return this.formatted(total);
  }

  public get totalProfit(): number {
    var amounts = this.records
      .map(item => item.amount)
      .map(item => item.profit);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get accountingFormattedProfit(): string {
    let profit = this.totalProfit;
    let result: string;

    if (profit >= 0) {
      profit = 0 - profit;
    }
    result = this.formatted(profit);

    if (profit >= 0) {
      return result;
    }

    return `(${result})`;
  }

  private formatted(value: number): string {
    return value.toLocaleString("en-CA", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
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
          <div className="credits number">{this.totalCredits}</div>
          <div className="debits number">{this.totalDebits}</div>
          <div className="profit number">{this.accountingFormattedProfit}</div>
        </div>
      </div>
    );
  }
}

export default DetailedValues;

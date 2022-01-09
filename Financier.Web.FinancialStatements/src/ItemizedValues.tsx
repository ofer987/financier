import * as React from "react";
import _ from "underscore";

import ItemizedValue from "./ItemizedValue";
import { ItemizedRecord } from "./ItemizedRecord";

interface Props {
  records: ItemizedRecord[];
}

class ItemizedValues extends React.Component<Props> {
  public get records(): ItemizedRecord[] {
    return this.props.records;
  }

  public get totalCredits(): string {
    let amounts = this.records
      .map(item => item.amount)
      .map(item => item.credit);

    const total = _.reduce(amounts, (total, amount) => total + amount) || 0;
    return this.formatted(total);
  }

  public get totalDebits(): string {
    var amounts = this.records
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

  public toKey(record: ItemizedRecord): string {
    return `${record.name}-${record.at}`;
  }

  render() {
    return (
      <div className="itemized-values">
        <h2>Items</h2>
        <div className="header">
          <div className="at">At</div>
          <div className="name">Name</div>
          <div className="tags">Tags</div>
          <div className="credits number">Credits</div>
          <div className="debits number">Debits</div>
          <div className="profit number">Profit (Deficit)</div>
        </div>
        <div className="items">
          {this.records.map(item => <ItemizedValue record={item} key={this.toKey(item)} />)}
        </div>
        <div className="total">
          <div className="at">Total</div>
          <div className="name"></div>
          <div className="tags"></div>
          <div className="credits number">{this.totalCredits}</div>
          <div className="debits number">{this.totalDebits}</div>
          <div className="profit number">{this.accountingFormattedProfit}</div>
        </div>
      </div>
    );
  }
}

export default ItemizedValues;

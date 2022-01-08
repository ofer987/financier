import * as React from "react";
import _ from "underscore";

import ItemizedValue from "./ItemizedValue";
import { ItemizedRecord } from "./ItemizedRecord";

interface Props {
  records: ItemizedRecord[];
}

class ItemizedValues extends React.Component<Props> {
  decimalCount = 2;

  public get records(): ItemizedRecord[] {
    return _.sortBy(this.props.records, item => item.at);
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

  public get totalProfit(): number {
    var amounts = this.records
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
          <div className="credits number">{this.totalCredits.toFixed(this.decimalCount)}</div>
          <div className="debits number">{this.totalDebits.toFixed(this.decimalCount)}</div>
          <div className="profit number">{this.accountingFormattedProfit}</div>
        </div>
      </div>
    );
  }
}

export default ItemizedValues;

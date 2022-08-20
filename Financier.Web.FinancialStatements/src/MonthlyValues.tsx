import * as React from "react";
import _ from "underscore";

import MonthlyValue from "./MonthlyValue";
import { MonthlyRecord } from "./MonthlyRecord";

import "./MonthlyValues.scss";

interface Props {
  records: MonthlyRecord[];
}

class MonthlyValues extends React.Component<Props> {
  public get records(): MonthlyRecord[] {
    return this.props.records;
  }

  public get totalCredits(): string {
    const amounts = this.records
      .map(item => item.amount)
      .map(item => item.credit);

    const total =_.reduce(amounts, (total, amount) => total + amount) || 0;
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

    if (profit < 0) {
      profit = 0 - profit;
      return `(${this.formatted(profit)})`;
    }

    return this.formatted(profit);
  }

  public render() {
    return (
      <div className="MonthlyValues">
        <h2>Items</h2>
        <div className="header">
          <div className="name">When</div>
          <div className="credits number">Credits</div>
          <div className="debits number">Debits</div>
          <div className="profit number">Profit (Deficit)</div>
        </div>
        <div className="items">
          { /* TODO: Display credits and debits should be children of dates (ats) */ }
          { /*this.dates */ }
          {this.records
            .map(item => <MonthlyValue record={item} key={`${item.year}-${item.month}-${item.amount}`} />)}
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

  private formatted(value: number): string {
    return value.toLocaleString("en-CA", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }
}

export default MonthlyValues;

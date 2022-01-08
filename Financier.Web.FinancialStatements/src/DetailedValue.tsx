import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import { DetailedRecord } from "./DetailedRecord";

interface Props {
  year: number;
  month: number;
  record: DetailedRecord;
}

class DetailedValue extends React.Component<Props> {
  decimalCount = 2;

  public get year(): number {
    return this.props.year;
  }

  public get month(): number {
    return this.props.month;
  }

  public get tagNames(): string {
    return this.props.record.tags
      .map(item => item.trim())
      .join(",");
  }

  render() {
    return (
      <div className="item clickable" id={this.name} key={this.name} onClick={() => this.navigateToItemizedView(this.year, this.month)}>
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
      </div>
    )
  }

  get name(): string {
    return _.uniq(this.props.record.tags)
      .map(tag => lodash.startCase(tag))
      .join(", ");
  }

  get credit(): string {
    return this.props.record.amount.credit.toFixed(2);
  }

  get debit(): string {
    return this.props.record.amount.debit.toFixed(2);
  }

  get accountingFormattedProfit(): string {
    const profit = this.props.record.amount.profit;
    if (profit >= 0) {
      return profit.toFixed(this.decimalCount);
    }

    return `(${(0 - profit).toFixed(this.decimalCount)})`;
  }

  private navigateToItemizedView(year: number, month: number): void {
    window.location.pathname = `/itemized-view/year/${year}/month/${month}/tagNames/${this.tagNames}`;

    return;
  }
}

export default DetailedValue;

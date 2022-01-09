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
    return this.formatted(this.props.record.amount.credit);
  }

  get debit(): string {
    return this.formatted(this.props.record.amount.debit);
  }

  private get accountingFormattedProfit(): string {
    let profit = this.props.record.amount.profit;
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

  private navigateToItemizedView(year: number, month: number): void {
    window.location.pathname = `/itemized-view/year/${year}/month/${month}/tagNames/${this.tagNames}`;

    return;
  }
}

export default DetailedValue;

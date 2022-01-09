import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import { ItemizedRecord } from "./ItemizedRecord";

interface Props {
  record: ItemizedRecord;
}

class ItemizedValue extends React.Component<Props> {
  decimalCount = 2;

  render() {
    return (
      <div className="item" id={this.name} key={this.name}>
        <div className="at">
          {this.at}
        </div>
        <div className="name">
          {this.name}
        </div>
        <div className="tags">
          {this.tags}
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
    return this.props.record.name;
  }

  get at(): string {
    return this.props.record.at;
  }

  get tags(): string {
    return this.props.record.tags
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
}

export default ItemizedValue;

import * as React from "react";
import _  from "underscore";
import { startCase } from "lodash";
import { ItemizedRecord } from "./ItemizedRecord";

interface Props {
  record: ItemizedRecord;
}

class ItemizedValue extends React.Component<Props> {
  decimalCount = 2;

  render() {
    return (
      <div className="item" id={this.name} key={this.name}>
        <div className="name">
          {this.name}
        </div>
        <div className="tags">
          {this.tags}
        </div>
        <div className="credit">
          {this.credit}
        </div>
        <div className="debit">
          {this.debit}
        </div>
        <div className="profit">
          {this.accountingFormattedProfit}
        </div>
      </div>
    )
  }

  get name(): string {
    return this.props.record.name;
  }

  get tags(): string {
    return this.props.record.tags
      .map(tag => startCase(tag))
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
}

export default ItemizedValue;

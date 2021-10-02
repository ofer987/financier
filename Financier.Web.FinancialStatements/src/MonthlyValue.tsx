import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import { Listing } from "./Listing";

interface Props {
  at: Date;
  debit: Listing;
  credit: Listing;
}

class MonthlyValue extends React.Component<Props> {
  render() {
    return (
      <div className="item" id={this.name} key={this.name}>
        <div className="name">
          {this.name}
        </div>
        <div className="credit">
          {this.creditAmount}
        </div>
        <div className="debit">
          {this.debitAmount}
        </div>
        <div className="profit">
          {/* Left empty on purpose */}
        </div>
      </div>
    )
  }

  get name(): string {
    return `${this.props.at.getFullYear()} - ${this.props.at.getMonth() + 1}`;
  }

  get creditAmount(): string {
    return this.props.credit.amount.toFixed(2);
  }

  get debitAmount(): string {
    return this.props.debit.amount.toFixed(2);
  }
}

export default MonthlyValue;

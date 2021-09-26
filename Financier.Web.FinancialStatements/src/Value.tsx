import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import { Listing } from "./Listing";

interface Props {
  debit: Listing;
  credit: Listing;
}

class Value extends React.Component<Props> {
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
      </div>
    )
  }

  get name(): string {
    var tags = this.props.credit.tags.concat(
      this.props.debit.tags);

    return _.uniq(tags)
      .map(tag => lodash.startCase(tag))
      .join(", ");
  }

  get creditAmount(): string {
    return this.props.credit.amount.toFixed(2);
  }

  get debitAmount(): string {
    return this.props.debit.amount.toFixed(2);
  }
}

export default Value;

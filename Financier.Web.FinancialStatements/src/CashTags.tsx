import * as React from "react";
import { Listing } from "./Listing";

class Props {
  debit: Listing;
  credit: Listing;
}

class Value extends React.Component<Props> {
  render() {
    return (
      <div id={this.name} key={this.name}>
        <div className="name">
          {this.name}
        </div>
        <div className="credits">
          {this.props.credit.amount}
        </div>
        <div className="debit">
          {this.props.debit.amount}
        </div>
      </div>
    )
  }

  get name(): string {
    var tags = this.props.credit.tags.concat(
      this.props.debit.tags);

    return tags.join("-");
  }

  get profit(): number {
    return this.props.credit.amount - this.props.debit.amount;
  }
}

export default Value;

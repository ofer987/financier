import * as React from "react";
import _  from "underscore";
import Listing from "./Listing";

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

    return _.uniq(tags).join("-");
  }
}

export default Value;

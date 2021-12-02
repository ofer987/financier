import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import { Listing } from "./Listing";

interface Props {
  listing: Listing;
}

class DetailedValue extends React.Component<Props> {
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
    // var tags = this.props.listing.tags.concat(
    //   this.props.debit.tags);

    return _.uniq(this.props.listing.tags)
      .map(tag => lodash.startCase(tag))
      .join(", ");
  }

  get creditAmount(): string {
    return this.props.listing.creditAmount.toFixed(2);
  }

  get debitAmount(): string {
    return this.props.listing.debitAmount.toFixed(2);
  }
}

export default DetailedValue;

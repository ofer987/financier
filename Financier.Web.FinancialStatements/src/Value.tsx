import * as React from "react";
import Listing from "./Listing";
import CashFlowModel from "./CashFlowModel";

class Value extends React.Component<CashFlowModel> {
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

  get at(): string {
    return `${this.props.startAt.getFullYear()}-${this.props.endAt.getMonth()}`;
  }
}

export default CashTags;

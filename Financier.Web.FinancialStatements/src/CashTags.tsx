import * as React from "react";
import CashTagsModel from "./CashTagsModel";

class CashTags extends React.Component<CashTagsModel> {
  render() {
    return (
      <div id={this.name()} key={this.name()}>
        <div className="name">
          {this.name()}
        </div>
        <div className="amount">
          {this.props.amount}
        </div>
        <div className="at">
          {this.at()}
        </div>
      </div>
    )
  }

  name(): string {
    return this.props.tags
      .map(tag => tag.name)
      .join(", ");
  }

  at(): string {
    return `${this.props.startAt.getFullYear()}-${this.props.endAt.getMonth()}`;
  }
}

export default CashTags;

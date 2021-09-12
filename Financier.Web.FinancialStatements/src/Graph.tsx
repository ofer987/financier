import * as React from "react";
import CashTags from "./CashTags";
import CashTagsModel from "./CashTagsModel";

interface Props {
  debits: CashTagsModel[];
  credits: CashTagsModel[];
}

class Graph extends React.Component<Props> {
  render() {
    return (
      <div>
        <h2>This is a graph</h2>

        <div className="all-debits" key="all-debits">
          {this.props.debits.map(item => <CashTags key={this.createUniqueKey(item)} startAt={item.startAt} endAt={item.endAt} tags={item.tags} amount={item.amount} />)}
        </div>

        <div className="all-credits" key="all-credits">
          {this.props.credits.map(item => <CashTags key={this.createUniqueKey(item)} startAt={item.startAt} endAt={item.endAt} tags={item.tags} amount={item.amount} />)}
        </div>
      </div>
    );
  }

  private createUniqueKey(item: CashTagsModel): string {
    return item.tags
      .map(item => item.name)
      .join(", ");
  }
};

export { Graph };

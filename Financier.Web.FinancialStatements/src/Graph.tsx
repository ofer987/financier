import * as React from "react";
import CashFlowModel from "./CashFlowModel";
import ValueProps from "./ValueProps"

class Graph extends React.Component<ValueProps> {
  render() {
    return (
      <div>
        <h2>This is a graph</h2>
      </div>
    );
  }

  private createUniqueKey(item: CashFlowModel): string {
    return item.tags.join("-");
  }
};

export { Graph };

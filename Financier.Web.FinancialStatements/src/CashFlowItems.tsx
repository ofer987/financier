import * as React from "react";

import CashFlowItemModel from "./CashFlowItemModel";
import CashFlowItem from "./CashFlowItem";

interface Props {
  revenues: CashFlowItemModel[];
  expenses: CashFlowItemModel[];
}

interface State {
}

class CashFlowItems extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
  }

  render() {
    return (
      <div className="cash-flow">
        <div className="size">
          {this.props.revenues.length}
        </div>
        <ul className="revenues">
          {this.props.revenues.map(item => <CashFlowItem name={item.name} price={item.price} at={item.at} key={item.name} />)}
        </ul>

        <div className="expenses">
        </div>
      </div>
    )
  }
};

export default CashFlowItems;

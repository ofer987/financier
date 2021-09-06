import * as React from "react";

interface Props {
  name: string;
  price: number;
  at: Date;
}

function CashFlowItem(props: Props) {
  return (
    <li className={props.name} key={props.name}>
      <span>{props.name}</span>
      <span>{props.price}</span>
      <span>{props.at.toString()}</span>
    </li>
  );
}

export default CashFlowItem;

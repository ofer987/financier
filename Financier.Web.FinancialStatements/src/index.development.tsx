'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import CashFlowItemModel from "./CashFlowItemModel";
import CashFlowItems from "./CashFlowItems";

// CSS
import "./index.scss";

var revenues: CashFlowItemModel[] = [
  new CashFlowItemModel("Spider-man", 45.05, new Date(2020, 4, 2)),
  new CashFlowItemModel("Superman", 90.0, new Date(2020, 4, 3)),
];

var expenses: CashFlowItemModel[] = [
  new CashFlowItemModel("Mortgage", 750, new Date(2020, 4, 5)),
];

interface Props {
  name: string;
  children: React.ReactNode;
}

interface State {
}

class Welcome extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
  }

  render() {
    return (<h1>Hello World\n{this.props.name}{this.props.children}</h1>)
  }
}

// function Name(props: { name: string; }) {
//   return (
//     <p>{props.name}</p>
//   )
// }

var root = document.querySelector(".root");
ReactDOM.render(
  <Welcome name="Dan">
    <CashFlowItems revenues={revenues} expenses={expenses} />
  </Welcome>,
  root);

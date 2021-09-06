import * as React from "react";
import * as ReactDOM from "react-dom";
import CashFlowItemModel from "./CashFlowItemModel";

var items: CashFlowItemModel[] = [
    new CashFlowItemModel("Spider-man", 45.05, new Date(2020, 4, 2)),
    new CashFlowItemModel("Superman", 90.0, new Date(2020, 4, 3)),
];

class Welcome extends React.Component {
    render() {
        return (
            <h1>Hello World\n</h1>
        );
    }
}

var root = document.querySelector(".root");
ReactDOM.render((<div>Hi</div>), root);

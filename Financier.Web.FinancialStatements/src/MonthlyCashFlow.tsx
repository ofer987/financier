import * as React from 'react';

import CashFlowItems from "./CashFlowItems";
import CashFlowItemModel from "./CashFlowItemModel";

interface MonthlyCashFlowProps {
    revenues: Array<CashFlowItemModel>;
    expenses: Array<CashFlowItemModel>;
}

class MonthlyCashFlow extends React.Component<MonthlyCashFlowProps, {}> {
    render() {
        return (
            <div>
                <h2>Chart</h2>
                <h3>Revenues</h3>
                <CashFlowItems items={this.props.revenues} />

                <h3>Expenses</h3>
                <CashFlowItems items={this.props.expenses} />
            </div>
        );
    }
}

export { MonthlyCashFlow, MonthlyCashFlowProps };

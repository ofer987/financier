import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';

class CashFlowItem {
    private _name: string;
    private _price: number;
    private _at: Date;

    public get name(): string {
        return this._name;
    }

    public get price(): number {
        return this._price;
    }

    public get at(): Date {
        return this._at;
    }

    constructor(name: string, price: number, at: Date) {
        this._name = name;
        this._price = price;
        this._at = at;
    }
}

interface MonthlyCashFlowModel {
    revenues: Array<CashFlowItem>;
    expenses: Array<CashFlowItem>;
}

const MonthlyCashFlow: React.FC<MonthlyCashFlowModel> = (props) => {
    return (
        <div>
            <h2>Chart</h2>
            <h3>Revenues</h3>
            <CashFlowItems items={props.revenues} />

            <h3>Expenses</h3>
            <CashFlowItems items={props.expenses} />
        </div>
    );
};

interface CashFlowItemsProps {
    items: Array<CashFlowItem>;
}

const CashFlowItems: React.FC<CashFlowItemsProps> = (props) => {
    var itemElement = (item: CashFlowItem) => {
        (
            <li className={item.name}>
                <span>{item.name}</span> | <span>{item.price}</span> | <span>{item.at}</span>
            </li>
        )
    };

    return (
        <ul className="revenues">
            {props.items.forEach(itemElement)};
        </ul>
    );
};

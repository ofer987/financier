import * as React from 'react';

import CashFlowItemModel from "./CashFlowItemModel";

interface CashFlowItemsProps {
    items: Array<CashFlowItemModel>;
}

const CashFlowItems: React.FC<CashFlowItemsProps> = (props) => {
    var itemElement = (item: CashFlowItemModel) => {
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

export default CashFlowItems;

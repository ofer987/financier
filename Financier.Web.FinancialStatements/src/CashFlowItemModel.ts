class CashFlowItemModel {
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

export default CashFlowItemModel;

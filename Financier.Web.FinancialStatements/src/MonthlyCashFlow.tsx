'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3-time-format";
import MonthlyValues from "./MonthlyValues";
import MonthlyListing from "./MonthlyListing";
import { Listing, ExpenseTypes } from "./Listing";
import { MonthlyGraph, MonthlyProp } from "./MonthlyGraph";
import {
  ApolloClient,
  InMemoryCache,
  ApolloProvider,
  useQuery,
  gql
} from "@apollo/client";

// CSS
import "./index.scss";

interface Props {
  fromYear: number;
  fromMonth: number;
  toYear: number;
  toMonth: number;
}

interface CheckedTag {
  name: string;
  checked: boolean;
}

interface CashFlow {
  startAt: string;
  endAt: string;
  debitListings: {
    amount: number;
  }[];
  creditListings: {
    amount: number;
  }[];
}

interface CashFlowResponse {
  getMonthlyCashFlows: CashFlow[];
}

class MonthlyCashFlow extends React.Component<Props, CashFlowResponse> {
  get cashFlows(): CashFlow[] {
    if (!this.state) {
      return [];
    }

    if (!this.state.getMonthlyCashFlows) {
      return [];
    }

    return this.state.getMonthlyCashFlows;
  }

  get fromYear(): number {
    return this.props.fromYear;
  }

  get fromMonth(): number {
    return this.props.fromMonth;
  }

  get toYear(): number {
    return this.props.toYear;
  }

  get toMonth(): number {
    return this.props.toMonth;
  }

  private client = new ApolloClient({
    uri: "https://localhost:5003/graphql/cash-flows",
    cache: new InMemoryCache(),
    headers: {
      "Content-Type": "application/json"
    }
  });

  constructor(props: Props) {
    super(props);

    this.setData();
  }

  setData(): void {
    // Convert to async/await
    this.client.query<CashFlowResponse>({
      query: gql`
        query {
          getMonthlyCashFlows(fromYear: ${this.fromYear}, fromMonth: ${this.fromMonth}, toYear: ${this.toYear}, toMonth: ${this.toMonth}) {
            startAt
            endAt
            debitListings {
              amount
            }
            creditListings {
              amount
            }
          }
        }
      `
    }).then(value => {
      console.log(value.data.getMonthlyCashFlows.length);
      const credits = this.toCreditCashFlowModel(value.data);
      const debits = this.toDebitCashFlowModel(value.data);
      console.log(`1: ${credits.length}, ${debits.length}`);

      this.setState(value.data);
    });
  }

  // renderCriteria() {
  //   return (
  //     <div className="criteria">
  //       <h2>Please Select</h2>
  //       {
  //         this.tags().map(tag =>
  //         <div className="checkbox">
  //           <input id={`${tag}`} type="checkbox" name={tag} onClick={() => this.toggleTag(tag)} />
  //           <label htmlFor={`${tag}`}>{lodash.startCase(tag)}</label>
  //         </div>
  //         )
  //       }
  //     </div>
  //   )
  // }

  render() {
    return (
      <div className="cash-flow">
        <h3>{this.fromYear}</h3>
        <a href="/">Select Different Time Range</a>
        <div className="monthly-cashflow">
          <MonthlyGraph dates={this.cashFlowsByDate()} />
        </div>
        <MonthlyValues dates={this.cashFlowsByDate()} dateRange={this.dateRange()} />
      </div>
    );
  }

  private toDebitCashFlowModel(data: CashFlowResponse): Listing[] {
    const monthlyCashFlows = data.getMonthlyCashFlows;

    return monthlyCashFlows.map(cashFlow => {
      const date = this.toDate(cashFlow.startAt)
      const year = date.getFullYear();
      const month = date.getMonth();
      const amounts = cashFlow.debitListings.map(listing => listing.amount);

      let total = 0;
      total = _.reduce(amounts, (t, amount) => t + amount);

      return new MonthlyListing(year, month, total, ExpenseTypes.Debit);
    });
  }

  private toCreditCashFlowModel(data: CashFlowResponse): Listing[] {
    const monthlyCashFlows = data.getMonthlyCashFlows;

    return monthlyCashFlows.map(cashFlow => {
      const date = this.toDate(cashFlow.startAt)
      const year = date.getFullYear();
      const month = date.getMonth();
      const amounts = cashFlow.creditListings.map(listing => listing.amount);

      let total = 0;
      total = _.reduce(amounts, (t, amount) => t + amount);

      return new MonthlyListing(year, month, total, ExpenseTypes.Credit);
    });
  }

  private cashFlowsByDate(): MonthlyProp[] {
    return this.cashFlows.map(item => {
      const date = this.toDate(item.startAt)
      const year = date.getFullYear();
      const month = date.getMonth();
      const creditAmounts = item.creditListings.map(listing => listing.amount);
      const debitAmounts = item.debitListings.map(listing => listing.amount);

      const creditTotal = _.reduce(creditAmounts, (t, amount) => t + amount) || 0;
      const debitTotal = _.reduce(debitAmounts, (t, amount) => t + amount) || 0;

      return {
        at: date,
        credit: new MonthlyListing(year, month, creditTotal, ExpenseTypes.Credit),
        debit: new MonthlyListing(year, month, debitTotal, ExpenseTypes.Debit)
      }
    });
  }

  private dateRange(): [Date, Date] | undefined {
    const values = this.cashFlows;
    if (values.length == 0) {
      return undefined;
    }

    return [
      this.toDate(values[0].startAt),
      this.toDate(values[values.length - 1].startAt),
    ];
  }

  private toDate(input: string): Date {
    const parser = d3.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export { MonthlyCashFlow, Props };

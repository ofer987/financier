'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3-time-format";
import MonthlyValues from "./MonthlyValues";
import MonthlyCashFlowModel from "./MonthlyCashFlowModel";
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
  match: {
    params: {
      year: number;
    }
  }
}

interface CheckedTag {
  name: string;
  checked: boolean;
}

interface CashFlowResponse {
  getMonthlyCashFlows: [
    {
      startAt: string;
      endAt: string;
      debitListings: {
        amount: number;
      }[]
      creditListings: {
        amount: number;
      }[]
    }
  ]
}

class MonthlyCashFlow extends React.Component<Props, CashFlowResponse> {
  get year(): number {
    return this.props.match.params.year;
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

    // this.state = { 
    //   getMonthlyCashFlows: [{
    //     startAt: "",
    //     endAt: "",
    //     debitListings: {
    //       amount: 0
    //     }[],
    //     creditListings: {
    //       amount: 0
    //     }
    //   }]
    // };
    this.fetchData();
  }

  fetchData(): void {
    // Convert to async/await
    this.client.query<CashFlowResponse>({
      query: gql`
        query {
          getMonthlyCashFlows(year: ${this.year}) {
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
        <div className="better-together">
          <MonthlyGraph dates={this.toDates(this.state)} />
        </div>
        <MonthlyValues dates={this.toDates(this.state)} />
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

      return new MonthlyCashFlowModel(year, month, total, ExpenseTypes.Debit);
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

      return new MonthlyCashFlowModel(year, month, total, ExpenseTypes.Credit);
    });
  }

  private toDates(data: CashFlowResponse): MonthlyProp[] {
    if (!data) {
      return [];
    }

    const monthlyCashFlows = data.getMonthlyCashFlows;

    return monthlyCashFlows.map(item => {
      const date = this.toDate(item.startAt)
      const year = date.getFullYear();
      const month = date.getMonth();
      const creditAmounts = item.creditListings.map(listing => listing.amount);
      const debitAmounts = item.debitListings.map(listing => listing.amount);

      const creditTotal = _.reduce(creditAmounts, (t, amount) => t + amount);
      const debitTotal = _.reduce(debitAmounts, (t, amount) => t + amount);

      return {
        at: date,
        credit: new MonthlyCashFlowModel(year, month, creditTotal, ExpenseTypes.Credit),
        debit: new MonthlyCashFlowModel(year, month, debitTotal, ExpenseTypes.Debit)
      }
    });
  }

  private toDate(input: string): Date {
    const parser = d3.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export default MonthlyCashFlow;

'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as d3 from "d3-time-format";
import Values from "./Values";
import CashFlowModel from "./CashFlowModel";
import { Listing, ExpenseTypes } from "./Listing";
import { Graph } from "./Graph";
import {
  ApolloClient,
  InMemoryCache,
  ApolloProvider,
  useQuery,
  gql
} from "@apollo/client";

// CSS
import "./index.scss";

// 1. Use a service to retrieve CashFLowItems using GraphQL

interface Props {
  year: number;
  month: number;
}

class State {
  debits: Listing[];
  credits: Listing[];
}

interface CashFlowResponse {
  getMonthlyCashFlow: {
    startAt: string;
    endAt: string;
    debitListings: {
      tags: { name: string; }[];
      amount: number;
    }[]
    creditListings: {
      tags: { name: string; }[];
      amount: number;
    }[]
  }
}

class CashFlow extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = { debits: [], credits: [] };
    this.getData();
  }

  client = new ApolloClient({
    uri: "https://localhost:5003/graphql/cash-flows",
    cache: new InMemoryCache(),
    headers: {
      "Content-Type": "application/json"
    }
  });

  getData(): void {
    // Convert to async/await
    this.client.query<CashFlowResponse>({
      query: gql`
        query {
          getMonthlyCashFlow(year: ${this.props.year}, month: ${this.props.month}) {
            startAt
            endAt
            debitListings {
              tags {
                name
              }
              amount
            }
            creditListings {
              tags {
                name
              }
              amount
            }
          }
        }
      `
    }).then(value => {
      this.setState({
        debits: this.toDebitCashFlowModel(value.data),
        credits: this.toCreditCashFlowModel(value.data)
      });
    });
  }

  render() {
    return (
      <div className="cash-flow">
        <Graph debits={this.state.debits} credits={this.state.credits} />
        {/* <Criteria /> */}
        <Values debits={this.state.debits} credits={this.state.credits} />
      </div>
    );
  }

  private toDebitCashFlowModel(data: CashFlowResponse): CashFlowModel[] {
    var cashFlow = data.getMonthlyCashFlow;

    return cashFlow.debitListings.map(listing => new CashFlowModel(
      this.toDate(cashFlow.startAt),
      this.toDate(cashFlow.endAt),
      listing.tags,
      listing.amount,
      ExpenseTypes.Debit
    ));
  }

  private toCreditCashFlowModel(data: CashFlowResponse): CashFlowModel[] {
    var cashFlow = data.getMonthlyCashFlow;

    return cashFlow.creditListings.map(listing => new CashFlowModel(
      this.toDate(cashFlow.startAt),
      this.toDate(cashFlow.endAt),
      listing.tags,
      listing.amount,
      ExpenseTypes.Credit
    ));
  }

  private toDate(input: string): Date {
    const parser = d3.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export default CashFlow;

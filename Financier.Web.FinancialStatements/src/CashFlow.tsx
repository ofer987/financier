'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as d3 from "d3-time-format";
import CashFlowModel from "./CashFlowModel";
import CashTagsModel from "./CashTagsModel";
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

interface State {
  debits: CashTagsModel[];
  credits: CashTagsModel[];
}

interface MonthlyCashFlow {
  getMonthlyCashFlow: CashFlowModel;
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
    this.client.query<MonthlyCashFlow>({
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
        debits: this.getDebitTags(value.data.getMonthlyCashFlow),
        credits: this.getCreditTags(value.data.getMonthlyCashFlow)
      });
    });
  }

  render() {
    return (
      <Graph debits={this.state.credits} credits={this.state.debits} />
    );
  }

  private getDebitTags(data: CashFlowModel): CashTagsModel[] {
    return data.debitListings
          .map(listing => new CashTagsModel(this.toDate(data.startAt), this.toDate(data.endAt), listing.tags, listing.amount));
  }

  private getCreditTags(data: CashFlowModel): CashTagsModel[] {
    return data.creditListings
          .map(listing => new CashTagsModel(this.toDate(data.startAt), this.toDate(data.endAt), listing.tags, listing.amount));
  }

  private toDate(input: string): Date {
    const parser = d3.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export default CashFlow;

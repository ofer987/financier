'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import _ from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";
import {
  ApolloClient,
  InMemoryCache,
  ApolloProvider,
  useQuery,
  gql
} from "@apollo/client";

import ItemizedValues from "./ItemizedValues";
import { Amount } from "./Amount";
import { ItemizedRecord } from "./ItemizedRecord";
import { DetailedGraph } from "./DetailedGraph";

// CSS
import "./index.scss";

interface Props {
  year: number;
  month: number;
  tagNames: string[];
}

class State {
  records: ItemizedRecord[];
}

interface ItemResponse {
  itemsByTagNamesAndPostedAt: {
    id: string;
    description: string;
    postedAt: Date;
    amount: number;
  }[]
}

class ItemizedCashFlow extends React.Component<Props, State> {
  public get year(): number {
    return this.props.year;
  }

  public get month(): number {
    return this.props.month;
  }

  public get at(): Date {
    return new Date(this.year, this.month - 1, 1);
  }

  public get postedAt(): string {
    const year = d3TimeFormat.timeFormat("%Y")(this.at);
    const month = d3TimeFormat.timeFormat("%m")(this.at);

    return `"${year}-${month}-01"`;
  }

  public get tagNames(): string {
    const names = this.props.tagNames
      .map(item => `"${item}"`)
      .join(", ");

    return `[${names}]`;
  }

  public get records(): ItemizedRecord[] {
    return this.state.records;
  }

  private client = new ApolloClient({
    uri: "https://localhost:5003/graphql/items",
    cache: new InMemoryCache(),
    headers: {
      "Content-Type": "application/json"
    }
  });

  constructor(props: Props) {
    super(props);

    this.state = { records: [] };
    this.getData();
  }

  public sortedRecords(): ItemizedRecord[] {
    return lodash
      .sortBy(this.state.records, record => record.amount.profit)
      .reverse();
  }

  private getData(): void {
    // TODO Convert to async/await
    this.client.query<ItemResponse>({
      query: gql`
        query {
          itemsByTagNamesAndPostedAt(tagNames: ${this.tagNames}, postedAt: ${this.postedAt}) {
            id,
            description,
            postedAt
            amount
            itemId
          }
        }
      `
    }).then(value => {
      const records = this.toRecords(value.data);

      this.setState({
        records,
      });
    });
  }

  private renderDetailedNavigation(year: number, month: number) {
    return (
      <div className="button detailed-cashflow" key={`detailed-view-${year}-${month + 1}`} onClick={(event) => {
        event.preventDefault();
        window.location.pathname = `/detailed-view/year/${year}/month/${month}`;
      }}>
        Return to {d3TimeFormat.timeFormat(`%B %Y`)(this.at)} Detailed Charts    </div>
    );
  }

  public render() {
    return (
      <div className="cash-flow">
        <h2>Navigation</h2>
        <div className="time-navigation">
          <div className="button welcome" onClick={(event) => {
            event.preventDefault();
            window.location.pathname = "/";
          }}>
            Select a Different Time Range
          </div>
          {this.renderDetailedNavigation(this.year, this.month)}
        </div>
        <ItemizedValues records={this.sortedRecords()} />
      </div>
    );
  }

  private toRecords(data: ItemResponse): ItemizedRecord[] {
    const items = data.itemsByTagNamesAndPostedAt;

    let results: ItemizedRecord[] = items.map(item => {
      let amount: Amount;
      if (item.amount < 0) {
        amount = new Amount(0 - item.amount, 0);
      } else {
        amount = new Amount(0, item.amount);
      }

      return {
        name: item.description,
        at: item.postedAt,
        amount: amount,
        tags: this.props.tagNames
      };
    });

    return results;
  }
}

export { ItemizedCashFlow, Props };

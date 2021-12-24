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
  postedAt: Date;
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
    return this.props.postedAt.getFullYear();
  }

  public get month(): number {
    return this.props.postedAt.getMonth();
  }

  public get postedAt(): string {
    const year = d3TimeFormat.timeFormat("%Y")(this.props.postedAt);
    const month = d3TimeFormat.timeFormat("%m")(this.props.postedAt);

    return `${year}-${month}-01`;
  }

  public get tagNames(): string {
    return this.props.tagNames
      .map(item => `"${item}"`)
      .join(", ");
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
        window.location.pathname = `/detailed-view/from-year/${year}/from-month/1`;
      }}>
        Return to {d3TimeFormat.timeFormat(`%B %Y`)} Detailed Charts    </div>
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
        <ItemizedValues records={this.records} />
      </div>
    );
  }

  private toRecords(data: ItemResponse): ItemizedRecord[] {
    const items = data.itemsByTagNamesAndPostedAt;

    let results: ItemizedRecord[] = items.map(item => {
      return {
        name: item.description,
        at: item.postedAt,
        amount: new Amount(item.amount, 0),
        tags: this.props.tagNames
      };
    });

    return results;
  }
}

export { ItemizedCashFlow };

'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import _ from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";
import {
  ApolloClient,
  InMemoryCache,
  gql
} from "@apollo/client";

import ItemizedValues from "./ItemizedValues";
import { Amount } from "./Amount";
import { ItemizedRecord } from "./ItemizedRecord";
import * as Constants from "./components/api-authorization/ApiAuthorizationConstants"

// CSS
import "./index.scss";

interface Props {
  fromYear: number;
  fromMonth: number;
  toYear: number;
  toMonth: number;
}

class State {
  records: ItemizedRecord[];
}

interface ItemResponse {
  itemsByPostedAt: {
    id: string;
    description: string;
    postedAt: string;
    amount: number;
    tags: string[];
  }[]
}

class AllItemsCashFlow extends React.Component<Props, State> {
  public get fromYear(): number {
    return this.props.fromYear;
  }

  public get fromMonth(): number {
    return this.props.fromMonth;
  }

  public get fromDate(): Date {
    return new Date(this.fromYear, this.fromMonth - 1, 1);
  }

  public get fromPostedDate(): string {
    const year = d3TimeFormat.timeFormat("%Y")(this.fromDate);
    const month = d3TimeFormat.timeFormat("%m")(this.fromDate);

    return `"${year}-${month}-01"`;
  }

  public get toYear(): number {
    return this.props.toYear;
  }

  public get toMonth(): number {
    return this.props.toMonth;
  }

  public get toDate(): Date {
    return new Date(this.toYear, this.toMonth, 1);
  }

  public get toPostedDate(): string {
    const year = d3TimeFormat.timeFormat("%Y")(this.toDate);
    const month = d3TimeFormat.timeFormat("%m")(this.toDate);

    return `"${year}-${month}-01"`;
  }

  public get records(): ItemizedRecord[] {
    return this.state.records;
  }

  private client = new ApolloClient({
    uri: `https://localhost:${Constants.Port}/graphql/items`,
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
      .sortBy(this.state.records, record => record.at);
  }

  private getData(): void {
    // TODO Convert to async/await
    this.client.query<ItemResponse>({
      query: gql`
        query {
          itemsByPostedAt(fromDate: ${this.fromPostedDate}, toDate: ${this.toPostedDate}) {
            id,
            description,
            postedAt
            amount
            itemId
            tags
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
        Return to {d3TimeFormat.timeFormat(`%B %Y`)(this.fromDate)} Detailed Charts
      </div>
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
          {this.renderDetailedNavigation(this.fromYear, this.fromMonth)}
        </div>
        <ItemizedValues records={this.sortedRecords()} />
      </div>
    );
  }

  private toRecords(data: ItemResponse): ItemizedRecord[] {
    const items = data.itemsByPostedAt;

    let results: ItemizedRecord[] = items.map(item => {
      let amount: Amount;
      if (item.amount < 0) {
        amount = new Amount(0 - item.amount, 0);
      } else {
        amount = new Amount(0, item.amount);
      }

      return {
        id: item.id,
        name: item.description,
        at: item.postedAt,
        amount: amount,
        tags: lodash.sortBy(item.tags)
      };
    });

    return results;
  }
}

export { AllItemsCashFlow, Props };

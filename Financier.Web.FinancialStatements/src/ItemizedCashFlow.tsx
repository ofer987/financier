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
  tags: string[];
}

class State {
  records: ItemizedRecord[];
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

class DetailedCashFlow extends React.Component<Props, State> {
  public get year() {
    return this.props.year;
  }

  public get month() {
    return this.props.month;
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

    this.state = { records: [], checkedTags: [] };
    this.getData();
  }

  public getUncheckedTags(listings: ItemizedRecord[]): CheckedTag[] {
    const tags = listings.flatMap(listing => listing.tags);
    const names = _.uniq(tags);

    return names.map(name => {
      return {
        name,
        checked: false
      };
    });
  }

  public enabledTags(): string[] {
    // Return all tags
    if (this.state.checkedTags.filter(tag => tag.checked).length == 0) {
      return [];
    }

    return this.state.checkedTags
      .filter(tag => tag.checked)
      .map(tag => tag.name);
  }

  public sortedRecords(): ItemizedRecord[] {
    return lodash
      .sortBy(this.state.records, record => record.amount.profit)
      .reverse();
  }

  public enabledRecords(): ItemizedRecord[] {
    // Return all the records
    if (this.enabledTags().length == 0) {
      return this.sortedRecords();
    }

    // Return only the records that have the checked tag
    return this.sortedRecords().filter(record => {
      return record.tags
        .filter(recordTag => _.contains(this.enabledTags(), recordTag))
        .length > 0;
    });
  }

  public tags(): string[] {
    let results = this.state.checkedTags.map(tag => tag.name);
    results = _.uniq(results);
    results = _.sortBy(results);

    return results;
  }

  private getData(): void {
    // TODO Convert to async/await
    this.client.query<CashFlowResponse>({
      query: gql`
        query {
          getMonthlyCashFlow(year: ${this.year}, month: ${this.month}) {
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
      const records = this.toRecords(value.data);

      this.setState({
        records,
        checkedTags: this.getUncheckedTags(records)
      });
    });
  }

  private toggleTag(name: string): void {
    const currentTags = this.state.checkedTags;
    const selectedTag = currentTags.find(currentTag => currentTag.name == name);

    if (typeof (selectedTag) == "undefined") {
      return;
    }

    selectedTag.checked = !selectedTag.checked;

    this.setState({
      checkedTags: currentTags
    });
  }

  private renderCriteria() {
    return (
      <div className="criteria">
        <h2>Criteria</h2>
        {
          this.tags().map(tag =>
          <div className="checkbox" key={`checkbox-${tag}`}>
            <input id={`${tag}`} type="checkbox" name={tag} onClick={() => this.toggleTag(tag)} key={`checkbox-value-${tag}`} />
            <label htmlFor={`${tag}`} key={`checkbox-label-${tag}`}>{lodash.startCase(tag)}</label>
          </div>
        )
        }
      </div>
    )
  }

  private renderMonthlyNavigation(year: number) {
    return (
      <div className="button yearly-cashflow" key={`monthly-view-${year}`} onClick={(event) => {
        event.preventDefault();
        window.location.pathname = `/monthly-view/from-year/${year}/from-month/1/to-year/${year}/to-month/12`;
      }}>
        View the {year} Monthly Charts
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
          {this.renderMonthlyNavigation(this.year)}
        </div>
        <div className="detailed-cashflow">
          {this.renderCriteria()}
          <DetailedGraph records={this.enabledRecords()} />
        </div>
        <DetailedValues records={this.enabledRecords()}  />
      </div>
    );
  }

  private toRecords(data: CashFlowResponse): ItemizedRecord[] {
    var cashFlow = data.getMonthlyCashFlow;

    let results: ItemizedRecord[] = cashFlow.debitListings.map(item => {
      return {
        // TODO: Should the tags be unique and sorted?
        tags: item.tags.map(item => item.name),
        amount: new Amount(0, item.amount)
      };
    });

    cashFlow.creditListings.forEach(item => {
      const creditAmount = new Amount(
        item.amount,
        0
      );
      const creditTags = item.tags.map(item => item.name);;
      const creditTagsString = creditTags.join(", ");

      let doesRecordExist = false;
      for (let debitRecord of results) {
        const debitTagsString = debitRecord.tags.join(", ");
        if (debitTagsString == creditTagsString) {
          doesRecordExist = true;

          debitRecord.amount.credit = creditAmount.credit;
          break;
        }
      }

      if (!doesRecordExist) {
        results.push({
          tags: creditTags,
          amount: creditAmount
        });
      }
    });

    return results;
  }

  private toDate(input: string): Date {
    const parser = d3TimeFormat.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export { DetailedCashFlow };

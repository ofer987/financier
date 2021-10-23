'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import _ from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";
import DetailedValues from "./DetailedValues";
import DetailedListing from "./DetailedListing";
import { Listing, ExpenseTypes } from "./Listing";
import { DetailedGraph } from "./DetailedGraph";
import {
  ApolloClient,
  InMemoryCache,
  ApolloProvider,
  useQuery,
  gql
} from "@apollo/client";

// CSS
import "./index.scss";

// 1. Use a service to retrieve CashFlowItems using GraphQL

interface Props {
  year: number;
  month: number;
}

class State {
  debits: Listing[];
  credits: Listing[];
  tags: CheckedTag[];
}

interface CheckedTag {
  name: string;
  checked: boolean;
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

    console.log(props);
    this.state = { debits: [], credits: [], tags: [] };
    this.getData();
  }

  getAllTags(credits: Listing[], debits: Listing[]): CheckedTag[] {
    const tagsList = credits.map(listing => listing.tags).concat(debits.map(listing => listing.tags));

    let names = tagsList.flatMap(names => names);
    names = _.uniq(names);

    return names.map(name => {
      return {
        name,
        checked: false
      };
    });
  }

  enabledTags(): string[] {
    if (this.state.tags.filter(tag => tag.checked).length == 0) {
      return this.state.tags.map(tag => tag.name);
    }

    return this.state.tags
      .filter(tag => tag.checked)
      .map(tag => tag.name);
  }

  allTags(credits: Listing[], debits: Listing[]): string[][] {
    var creditTags = credits.map(item => item.tags);
    var debitTags = debits.map(item => item.tags);

    var tags = creditTags.concat(debitTags);

    return _.uniq(tags);
    return tags;
  }

  tags(): string[] {
    let results = this.allTags(this.state.credits, this.state.debits)
      .flatMap(tag => tag.flatMap(t => t));
    results = _.uniq(results);
    results = _.sortBy(results);

    return results;
  }

  getData(): void {
    // Convert to async/await
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
      const credits = this.toCreditCashFlowModel(value.data);
      const debits = this.toDebitCashFlowModel(value.data);

      this.setState({
        credits,
        debits,
        tags: this.getAllTags(credits, debits)
      });
    });
  }

  toggleTag(tag: string): void {
    const currentTags = this.state.tags;
    const currentTag = currentTags.find(currentTag => currentTag.name == tag);
    if (typeof (currentTag) != "undefined") {
      currentTag.checked = !currentTag.checked;

      this.setState({
        tags: currentTags
      })
    }
  }

  renderCriteria() {
    return (
      <div className="criteria">
        <h3>Please Select</h3>
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
      <div className="yearly-cashflow" key={`monthly-view-${year}`} onClick={(event) => {
        event.preventDefault();
        window.location.pathname = `/monthly-view/from-year/${year}/from-month/1/to-year/${year}/to-month/12`;
      }}>
        View the {year} Monthly Charts
      </div>
    );
  }

  render() {
    return (
      <div className="cash-flow">
        <h2>Detailed Chart</h2>
        <h3>Navigation</h3>
        <div className="navigation">
          <div className="welcome" onClick={(event) => {
            event.preventDefault();
            window.location.pathname = "/";
          }}>
            Select a Different Time Range
          </div>
          {this.renderMonthlyNavigation(this.year)}
        </div>
        <div className="detailed-cashflow">
          {this.renderCriteria()}
          <DetailedGraph debits={this.state.debits} credits={this.state.credits} enabledTags={this.enabledTags()} />
        </div>
        <DetailedValues debits={this.state.debits} credits={this.state.credits} enabledTags={this.enabledTags()} />
      </div>
    );
  }

  private toDebitCashFlowModel(data: CashFlowResponse): DetailedListing[] {
    var cashFlow = data.getMonthlyCashFlow;

    return cashFlow.debitListings.map(listing => new DetailedListing(
      this.toDate(cashFlow.startAt),
      this.toDate(cashFlow.endAt),
      listing.tags,
      listing.amount,
      ExpenseTypes.Debit
    ));
  }

  private toCreditCashFlowModel(data: CashFlowResponse): DetailedListing[] {
    var cashFlow = data.getMonthlyCashFlow;

    return cashFlow.creditListings.map(listing => new DetailedListing(
      this.toDate(cashFlow.startAt),
      this.toDate(cashFlow.endAt),
      listing.tags,
      listing.amount,
      ExpenseTypes.Credit
    ));
  }

  private toDate(input: string): Date {
    const parser = d3TimeFormat.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export default DetailedCashFlow;

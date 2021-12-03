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
  listings: Listing[];
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

    this.state = { debits: [], credits: [], tags: [] };
    this.getData();
  }

  getAllTags(listings: Listing[]): CheckedTag[] {
    const tags = listings.flatMap(listing => listing.tags);
    const names = _.uniq(tags);

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
      const listings = this.toListings(value.data);
      // const credits = this.toCreditCashFlowModel(value.data);
      // const debits = this.toDebitCashFlowModel(value.data);

      this.setState({
        listings,
        tags: this.getAllTags(listings)
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

  render() {
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
          <DetailedGraph listings={this.state.listings} enabledTags={this.enabledTags()} />
        </div>
        <DetailedValues listings={this.state.listings}  enabledTags={this.enabledTags()} />
      </div>
    );
  }

  private toListings(data: CashFlowResponse): DetailedListing[] {
    var cashFlow = data.getMonthlyCashFlow;

    let results = cashFlow.debitListings.map(item => new DetailedListing(
      item.tags,
      0,
      item.amount,
    ));

    cashFlow.creditListings.forEach(item => {
      let creditListing = new DetailedListing(
        item.tags,
        item.amount,
        0
      );

      let doesListingExist = false;
      for (let listing of results) {
        if (listing.toString() == creditListing.toString()) {
          doesListingExist = true;

          listing.creditAmount = creditListing.creditAmount;
          break;
        }
      }

      if (!doesListingExist) {
        results.push(creditListing);
      }
    });

    return results;
  }

  // private toDebitCashFlowModel(data: CashFlowResponse): DetailedListing[] {
  //   var cashFlow = data.getMonthlyCashFlow;
  //
  //   return cashFlow.debitListings.map(listing => new DetailedListing(
  //     this.toDate(cashFlow.startAt),
  //     this.toDate(cashFlow.endAt),
  //     listing.tags,
  //     listing.amount,
  //     ExpenseTypes.Debit
  //   ));
  // }
  //
  // private toCreditCashFlowModel(data: CashFlowResponse): DetailedListing[] {
  //   var cashFlow = data.getMonthlyCashFlow;
  //
  //   return cashFlow.creditListings.map(listing => new DetailedListing(
  //     this.toDate(cashFlow.startAt),
  //     this.toDate(cashFlow.endAt),
  //     listing.tags,
  //     listing.amount,
  //     ExpenseTypes.Credit
  //   ));
  // }

  private toDate(input: string): Date {
    const parser = d3TimeFormat.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export default DetailedCashFlow;

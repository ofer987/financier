'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import _ from "underscore";
import lodash from "lodash";
import * as d3TimeFormat from "d3-time-format";
import DetailedValues from "./DetailedValues";
import { Listing } from "./Listing";
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
  listings: DetailedListing[];
  checkedTags: CheckedTag[];
}

interface CheckedTag {
  name: string;
  checked: boolean;
}

interface DetailedListing {
  tags: string[];
  listing: Listing;
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

    this.state = { listings: [], checkedTags: [] };
    this.getData();
  }

  public getUncheckedTags(listings: DetailedListing[]): CheckedTag[] {
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
      return this.state.checkedTags.map(tag => tag.name);
    }

    return this.state.checkedTags
      .filter(tag => tag.checked)
      .map(tag => tag.name);
  }

  public enabledListings(): DetailedListing[] {
    const enabledTags = this.enabledTags();

    return this.state.listings.filter(item => {
      item.tags.forEach(item1 => {
        enabledTags.forEach(item2 => {
          if (item1 == item2) {
            return true;
          }
        });

        return false;
      })
    });
    //   enabledTags (tag => item.tags.filter(t => t == tag))
    // });
    //   lodash.has.contains(item.tags, )
    //   item.tags
    // });
    // this.enabledTags().
  }

  // public allTags(listings: DetailedListing[]): string[] {
  //   const tags = listings.flatMap(listing => listing.tags);
  //   const names = _.uniq(tags);
  //
  //   return names;
  // }

  public tags(): string[] {
    let results = this.state.checkedTags.map(tag => tag.name);
    results = _.uniq(results);
    results = _.sortBy(results);

    return results;
  }

  private getData(): void {
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
        checkedTags: this.getUncheckedTags(listings)
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
          <DetailedGraph {...this.enabledListings()} />
        </div>
        <DetailedValues {...this.enabledListings()}  />
      </div>
    );
  }

  private toListings(data: CashFlowResponse): DetailedListing[] {
    var cashFlow = data.getMonthlyCashFlow;

    let results = cashFlow.debitListings.map(item => {
      return {
        // Should the tags be unique and sorted?
        tags: item.tags.map(item => item.name),
        listing: new Listing(0, item.amount)
      };
    });

    cashFlow.creditListings.forEach(item => {
      const creditListing = new Listing(
        item.amount,
        0
      );
      const creditTags = item.tags.map(item => item.name);;
      const creditTagsString = creditTags.join(", ");

      let doesListingExist = false;
      for (let debitListing of results) {
        const debitTagsString = debitListing.tags.join(", ");
        if (debitTagsString == creditTagsString) {
          doesListingExist = true;

          debitListing.listing.creditAmount = creditListing.creditAmount;
          break;
        }
      }

      if (!doesListingExist) {
        results.push({
          tags: creditTags,
          listing: creditListing
        });
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

export { DetailedCashFlow, DetailedListing };

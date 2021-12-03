'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3-time-format";
import {
  ApolloClient,
  InMemoryCache,
  ApolloProvider,
  useQuery,
  gql
} from "@apollo/client";

import MonthlyValues from "./MonthlyValues";
import NullListing from './NullListing';
import { Listing } from "./Listing";
import { MonthlyGraph, MonthlyProp } from "./MonthlyGraph";

// CSS
import "./index.scss";

interface Props {
  fromYear: number;
  fromMonth: number;
  toYear: number;
  toMonth: number;
}

// interface CheckedTag {
//   name: string;
//   checked: boolean;
// }

interface CashFlow {
  startAt: string;
  endAt: string;
  debitListings: {
    amount: number;
  }[];
  creditListings: {
    amount: number;
  }[];
}

interface CashFlows {
  getMonthlyCashFlows: CashFlow[];
}

interface MonthlyListing {
  year: number;
  month: number;
  listing: Listing;
}

class MonthlyCashFlow extends React.Component<Props, MonthlyListing[]> {
  get listings(): MonthlyListing[] {
    return this.state.map(v => v);
  }

  get fromYear(): number {
    return lodash.toNumber(this.props.fromYear);
  }

  get fromMonth(): number {
    return lodash.toNumber(this.props.fromMonth);
  }

  get toYear(): number {
    return lodash.toNumber(this.props.toYear);
  }

  get toMonth(): number {
    return lodash.toNumber(this.props.toMonth);
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

    this.state = [];
    this.setData();
  }

  setData(): void {
    // Convert to async/await
    this.client.query<CashFlows>({
      query: gql`
        query {
          getMonthlyCashFlows(fromYear: ${this.fromYear}, fromMonth: ${this.fromMonth}, toYear: ${this.toYear}, toMonth: ${this.toMonth}) {
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
      const listings = this.toListings(value.data);

      // const credits = this.toCreditCashFlowModel(value.data);
      // const debits = this.toDebitCashFlowModel(value.data);

      this.setState(listings);
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

  public navigatetoDifferentRange(event: Event) {
    event.preventDefault();

    window.location.pathname = "/";
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
          <div className="yearly-navigation">
            {lodash.range(this.fromYear, this.toYear + 1, 1).map((year: number) => {
              return this.renderMonthlyNavigation(year);
            })}
          </div>
        </div>
        <div className="monthly-cashflow">
          <MonthlyGraph {...this.listings} />
        </div>
        <MonthlyValues {...this.listings} />
      </div>
    );
  }

  private toListings(values: CashFlows): MonthlyListing[] {
    const data = values.getMonthlyCashFlows.map(item => { 
      const at = this.toDate(item.startAt);
      const creditAmounts = item.creditListings.map(listing => listing.amount);
      const debitAmounts = item.debitListings.map(listing => listing.amount);

      let creditAmount = 0;
      creditAmount = _.reduce(creditAmounts, (t, amount) => t + amount);

      let debitAmount = 0;
      debitAmount = _.reduce(creditAmounts, (t, amount) => t + amount);

      return {
        year: at.getFullYear(),
        month: at.getMonth(),
        listing: new Listing(
          creditAmount,
          debitAmount
        )
      };
    });

    // create the results
    const results: MonthlyListing[] = [];
    const startAt = new Date(this.fromYear, this.fromMonth);
    const endAt = new Date(this.toYear, this.toMonth);
    for (let at = startAt; at <= endAt; at = new Date(at.setMonth(at.getMonth() + 1))) {
      const listing = data.find(item => item.year == at.getFullYear() && item.month == at.getMonth())
      if (typeof (listing) == "undefined") {
        results.push({
          year: at.getFullYear(),
          month: at.getMonth(),
          listing: new Listing(0, 0)
        });
      } else {
        results.push(listing);
      }
    }

    // Trim the results from the start
    const firstIndex = results.findIndex(item => item.listing.creditAmount != 0 && item.listing.debitAmount != 0);
    const lastIndex = _.findLastIndex(results, (item => item.listing.creditAmount != 0 && item.listing.debitAmount != 0));

    return results.slice(firstIndex, lastIndex);
  }
  // private toDebitCashFlowModel(data: CashFlowResponse): Listing[] {
  //   const monthlyCashFlows = data.getMonthlyCashFlows;
  //
  //   return monthlyCashFlows.map(cashFlow => {
  //     const date = this.toDate(cashFlow.startAt)
  //     const year = date.getFullYear();
  //     const month = date.getMonth();
  //     const amounts = cashFlow.debitListings.map(listing => listing.amount);
  //
  //     let total = 0;
  //     total = _.reduce(amounts, (t, amount) => t + amount);
  //
  //     return new MonthlyListing(year, month, total, ExpenseTypes.Debit);
  //   });
  // }

  // private toCreditCashFlowModel(data: CashFlowResponse): Listing[] {
  //   const monthlyCashFlows = data.getMonthlyCashFlows;
  //
  //   return monthlyCashFlows.map(cashFlow => {
  //     const date = this.toDate(cashFlow.startAt)
  //     const year = date.getFullYear();
  //     const month = date.getMonth();
  //     const amounts = cashFlow.creditListings.map(listing => listing.amount);
  //
  //     let total = 0;
  //     total = _.reduce(amounts, (t, amount) => t + amount);
  //
  //     return new MonthlyListing(year, month, total, ExpenseTypes.Credit);
  //   });
  // }

  // private cashFlowsByDate(): MonthlyProp[] {
  //   return this.cashFlows.map(item => {
  //     const date = this.toDate(item.startAt)
  //     const year = date.getFullYear();
  //     const month = date.getMonth();
  //     const creditAmounts = item.creditListings.map(listing => listing.amount);
  //     const debitAmounts = item.debitListings.map(listing => listing.amount);
  //
  //     const creditTotal = _.reduce(creditAmounts, (t, amount) => t + amount) || 0;
  //     const debitTotal = _.reduce(debitAmounts, (t, amount) => t + amount) || 0;
  //
  //     if (creditTotal == 0 && debitTotal == 0) {
  //       return {
  //         at: date,
  //         credit: new NullListing(),
  //         debit: new NullListing()
  //       };
  //     }
  //
  //     return {
  //       at: date,
  //       credit: new MonthlyListing(year, month, creditTotal, ExpenseTypes.Credit),
  //       debit: new MonthlyListing(year, month, debitTotal, ExpenseTypes.Debit)
  //     }
  //   });
  // }

  // private dateRange(): [Date, Date] | undefined {
  //   const values = this.cashFlows;
  //   if (values.length == 0) {
  //     return undefined;
  //   }
  //
  //   return [
  //     this.toDate(values[0].startAt),
  //     this.toDate(values[values.length - 1].startAt),
  //   ];
  // }

  private toDate(input: string): Date {
    const parser = d3.timeParse("%Y-%m-%dT%H:%M:%S");

    return parser(input);
  }
}

export { MonthlyCashFlow, MonthlyListing, Props };

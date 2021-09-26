'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import _ from "underscore";
import lodash from "lodash";
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

class CashFlow extends React.Component<Props, State> {
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
      // alert(`No tags have been selected`);

      return this.state.tags.map(tag => tag.name);
    }

    // alert(`one tag has been selected`);
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
    // alert(`Tag has been toggled: ${tag}`);

    const currentTags = this.state.tags;
    const currentTag = currentTags.find(currentTag => currentTag.name == tag);
    if (typeof (currentTag) != "undefined") {
      // alert(`Updating the tag ${tag}`);
      currentTag.checked = !currentTag.checked;
      // currentTags[currentTag.name] = !currentTag.checked;

      this.setState({
        tags: currentTags
      })
    }
  }

  renderCriteria() {
    return (
      <div className="criteria">
        <h2>Please Select</h2>
        {
          this.tags().map(tag =>
          <div className="checkbox">
            <input id={`${tag}`} type="checkbox" name={tag} onClick={() => this.toggleTag(tag)} />
            <label htmlFor={`${tag}`}>{lodash.startCase(tag)}</label>
          </div>
          )
        }
      </div>
    )
  }

  render() {
    return (
      <div className="cash-flow">
        <div className="better-together">
          {this.renderCriteria()}
          <Graph debits={this.state.debits} credits={this.state.credits} enabledTags={this.enabledTags()} />
        </div>
        <Values debits={this.state.debits} credits={this.state.credits} enabledTags={this.enabledTags()} />
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

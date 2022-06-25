import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import {
  ApolloClient,
  InMemoryCache,
  gql
} from "@apollo/client";

import { ItemizedRecord } from "./ItemizedRecord";
import * as Constants from "./Constants";

import "./ItemizedValue.scss";

interface Props {
  record: ItemizedRecord;
}

interface State {
  areTagsInteractive: boolean;
}

class ItemizedValue extends React.Component<Props, State> {
  private client = new ApolloClient({
    uri: `https://localhost:${Constants.Port}/graphql/items`,
    cache: new InMemoryCache(),
    headers: {
      "Content-Type": "application/json"
    }
  });

  private get areTagsInteractive(): boolean {
    return this.state?.areTagsInteractive ?? false;
  }

  private set areTagsInteractive(value: boolean) {
    this.setState({
      areTagsInteractive: value
    });
  }

  // private toggleInteractiveTags(event: any): void {
  //   event.preventDefault();
  //
  //   debugger;
  //   this.areTagsInteractive = !this.areTagsInteractive;
  // }

  constructor(props: Props) {
    super(props);

    this.state = {
      areTagsInteractive: false
    }
  }

  render() {
    return (
      <div className="ItemizedValue" id={this.name} key={this.key} onMouseEnter={_event => this.areTagsInteractive = true} onMouseLeave={_event => this.areTagsInteractive = false}>
        <div className="item">
          <div className="at">
            {this.at}
          </div>
          <div className="name">
            {this.name}
          </div>
          <input id={`${this.key}-div`} className={`tags ${this.areTagsInteractive ? "interactive" : ""}`} type="text" defaultValue={this.tags} onKeyDown={event => {
            if (event.key !== "Enter") {
              return;
            }

            event.preventDefault();

            let value = event.currentTarget.value.trim();
            let newTags = value.split(",")
          }} />
          <div className="credit number">
            {this.credit}
          </div>
          <div className="debit number">
            {this.debit}
          </div>
          <div className="profit number">
            {this.accountingFormattedProfit}
          </div>
        </div>
      </div>
    )
  }

  get id(): string {
    return this.props.record.id;
  }

  get key(): string {
    return `${this.id}-${this.name}-${this.at}`;
  }

  get name(): string {
    return this.props.record.name;
  }

  get at(): string {
    return this.props.record.at;
  }

  get tags(): string {
    return this.props.record.tags
      .map(tag => lodash.startCase(tag))
      .join(", ");
  }

  get credit(): string {
    return this.formatted(this.props.record.amount.credit);
  }

  get debit(): string {
    return this.formatted(this.props.record.amount.debit);
  }

  private get accountingFormattedProfit(): string {
    let profit = this.props.record.amount.profit;

    if (profit < 0) {
      profit = 0 - profit;
      return `(${this.formatted(profit)})`;
    }

    return this.formatted(profit);
  }

  private formatted(value: number): string {
    return value.toLocaleString("en-CA", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  private changeTags(value: string[], success: Function): void {
    this.client.mutate<boolean>({
      mutation: gql`
        mutation($itemId: ID!, $tagNames: [String]!) {
          updateTags(itemId: $itemId, newTags: $tagNames)
        }
      `,
      variables: {
        itemId: this.id,
        tagNames: value,
      }
    }).catch(error => {
      alert(error);
    }).then(_result => {
      alert(`success changing to tags: ${value}`);

      success(value);
    });
  }
}

export default ItemizedValue;

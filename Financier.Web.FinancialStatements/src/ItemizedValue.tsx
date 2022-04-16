import * as React from "react";
import _  from "underscore";
import lodash from "lodash";
import {
  ApolloClient,
  InMemoryCache,
  gql
} from "@apollo/client";

import { ItemizedRecord } from "./ItemizedRecord";

interface Props {
  record: ItemizedRecord;
}

interface State {
  areTagsInteractive: boolean;
}

class ItemizedValue extends React.Component<Props, State> {
  private client = new ApolloClient({
    uri: "https://localhost:7240/graphql/items",
    cache: new InMemoryCache(),
    headers: {
      "Content-Type": "application/json"
    }
  });

  private get areTagsInteractive(): boolean {
    return this.state.areTagsInteractive;
  }

  private set areTagsInteractive(value: boolean) {
    this.setState({
      areTagsInteractive: value
    });
  }

  constructor(props: Props) {
    super(props);

    this.state = {
      areTagsInteractive: false
    };
  }

  render() {
    return (
      <div className="item" id={this.name} key={this.key}>
        <div className="at">
          {this.at}
        </div>
        <div className="name">
          {this.name}
        </div>
        <div key={`${this.key}-div`} className={`tags non-interactive ${this.areTagsInteractive ? "none": "displayed"}`} onMouseMove={event => {
          event.preventDefault();

          this.areTagsInteractive = true;
        }}>
          {this.tags}
        </div>
        <input key={`${this.key}-input`} type="text" defaultValue={this.tags} className={`tags interactive ${this.areTagsInteractive ? "displayed": "none"}`} onMouseOut={_event => this.areTagsInteractive = false} onKeyDown={event => {
          if (event.code == "Enter") {
            let newTagsString = event.currentTarget.value.trim();

            // Validate the tags
            if (newTagsString == "") {
              alert(`new tags are empty. Reverting to ${this.tags}`);
            }

            const newTags = newTagsString.split(",")
              .map(item => item.trim())
              .map(item => lodash.kebabCase(item));

            this.changeTags(newTags, (value: string) => {
              event.currentTarget.textContent = value;
            });

            this.areTagsInteractive = false;
          }
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
    )
  }

  get id(): string {
    return this.props.record.id;
  }

  get key(): string {
    return `${this.name}-${this.at}`;
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

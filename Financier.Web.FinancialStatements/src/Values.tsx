import * as React from "react";
import _ from "underscore";
import Value from "./Value";
import { Listing } from "./Listing";
import NullListing from "./NullListing";

interface Props {
  debits: Listing[];
  credits: Listing[];
  enabledTags: string[];
}

class Values extends React.Component<Props> {
  constructor(props: Props) {
    super(props);
  }

  get enabledTags(): string[][] {
    const creditTags = this.props.credits.map(item => item.tags);
    const debitTags = this.props.debits.map(item => item.tags);

    let tagsList = creditTags.concat(debitTags);
    tagsList = _.uniq(tagsList);
    tagsList = tagsList.filter(tags => {
      return tags.find(tag => typeof (this.enabledTagNames.find(t => t == tag)) != "undefined");
    });

    return tagsList;
  }

  get enabledTagNames(): string[] {
    return this.props.enabledTags;
  }

  get totalCredits(): number {
    var amounts = this.props.credits
      .filter(credit => {
        const searchedTag = credit.tags
          .find(tag => _.contains(this.props.enabledTags, tag));

        return typeof (searchedTag) != "undefined";
      })
      .map(item => item.amount);
    var amount = 0;

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  get totalDebits(): number {
    var amounts = this.props.debits
      .filter(debit => {
        const searchedTag = debit.tags
          .find(tag => _.contains(this.props.enabledTags, tag));

        return typeof (searchedTag) != "undefined";
      })
      .map(item => item.amount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  findListingByTags(models: Listing[], tags: string[]): Listing {
    var result = models.find(item => item.tags == tags);

    if (typeof (result) == "undefined") {
      return new NullListing();
    }

    return result;
  }

  render() {
    return (
      <div className="values">
        <div className="header">
          <div className="name">Name</div>
          <div className="credits">Credits</div>
          <div className="debits">Debits</div>
        </div>
        <div className="items">
          {this.enabledTags.map(tags => <Value debit={this.findListingByTags(this.props.debits, tags)} credit={this.findListingByTags(this.props.credits, tags)} />)}
        </div>
        <div className="total">
          <div className="name">Total</div>
          <div className="credits">{this.totalCredits.toFixed(2)}</div>
          <div className="debits">{this.totalDebits.toFixed(2)}</div>
        </div>
      </div>
    );
  }
}

export default Values;

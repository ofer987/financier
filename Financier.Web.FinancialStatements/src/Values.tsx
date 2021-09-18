import * as React from "react";
import _, { uniq } from "underscore";
import Value from "./Value";
import Listing from "./Listing";
import CashTagsModel from "./CashTagsModel";
import ValueProps from "./CashFlowProps"
import NullListing from "./NullListing";

class Values extends React.Component<ValueProps> {

  get allTags(): string[][] {
    var creditTags = this.props.credits.map(item => item.tags);
    var debitTags = this.props.debits.map(item => item.tags);

    var tags = creditTags.concat(debitTags);

    return _.uniq(tags);
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
        {this.allTags.map(tags => <Value debit={this.findListingByTags(this.props.debits, tags)} credits={this.findListingByTags(this.props.credits, tags)} />
        )}
      </div>
    );
  }
}

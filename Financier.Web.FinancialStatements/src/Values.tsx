import * as React from "react";
import _ from "underscore";
import Value from "./Value";
import Listing from "./Listing";
import NullListing from "./NullListing";

interface Props {
  debits: Listing[];
  credits: Listing[];
}

class Values extends React.Component<Props> {
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
        <div className="name">Name</div>
        <div className="credits">Credits</div>
        <div className="Debits">Debits</div>
        {this.allTags.map(tags => <Value debit={this.findListingByTags(this.props.debits, tags)} credit={this.findListingByTags(this.props.credits, tags)} />
        )}
      </div>
    );
  }
}

export default Values;

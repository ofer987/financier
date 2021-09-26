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

abstract class FilterableController extends React.Component<Props> {
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
}

export default FilterableController;

import * as React from "react";
import _ from "underscore";

import DetailedValue from "./DetailedValue";
import { Listing } from "./Listing";
import { DetailedListing } from "./DetailedCashFlow";
// import NullListing from "./NullListing";

// interface Props {
//   listings: Listing[];
//   enabledTags: string[];
// }

abstract class FilterableController extends React.Component<DetailedListing[]> {
  constructor(props: DetailedListing[]) {
    super(props);
  }

  // get enabledListings(): DetailedListing[] {
  //   return this.getEnabledListings(this.props.listings);
  // }

  // get enabledCredits(): Listing[] {
  //   return this.getEnabledListings(this.props.credits);
  // }
  //
  // get enabledDebits(): Listing[] {
  //   return this.getEnabledListings(this.props.debits);
  // }

  // get enabledTags(): string[][] {
  //   let tagsList = this.props.listings.map(item => item.tags);
  //   // const creditTags = this.props.credits.map(item => item.tags);
  //   // const debitTags = this.props.debits.map(item => item.tags);
  //
  //   // let tagsList = creditTags.concat(debitTags);
  //   tagsList = _.uniq(tagsList);
  //   tagsList = tagsList.filter(tags => {
  //     return tags.find(tag => typeof (this.enabledTagNames.find(t => t == tag)) != "undefined");
  //   });
  //
  //   return tagsList;
  // }

  // get enabledTagNames(): string[] {
  //   return this.props.enabledTags;
  // }

  public get totalCredits(): number {
    var amounts = this.props
      .map(item => item.listing)
      .map(item => item.creditAmount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  public get totalDebits(): number {
    var amounts = this.props
      .map(item => item.listing)
      .map(item => item.debitAmount);

    return _.reduce(amounts, (total, amount) => total + amount) || 0;
  }

  // get totalCredits(): number {
  //   var amounts = this.props.listings
  //     .filter(item => {
  //       const searchedTag = item.tags
  //         .find(tag => _.contains(this.props.enabledTags, tag));
  //
  //       return typeof (searchedTag) != "undefined";
  //     })
  //     .map(item => item.amount);
  //   var amount = 0;
  //
  //   return _.reduce(amounts, (total, amount) => total + amount) || 0;
  // }
  //
  // get totalDebits(): number {
  //   var amounts = this.props.debits
  //     .filter(debit => {
  //       const searchedTag = debit.tags
  //         .find(tag => _.contains(this.props.enabledTags, tag));
  //
  //       return typeof (searchedTag) != "undefined";
  //     })
  //     .map(item => item.amount);
  //
  //   return _.reduce(amounts, (total, amount) => total + amount) || 0;
  // }

  // protected findListingByTags(models: DetailedListing[], tags: string[]): DetailedListing {
  //   var result = models.find(item => item.tags == tags);
  //
  //   return result;
  // }

  // private getEnabledListings(listings: Listing[]): Listing[] {
  //   return listings
  //     .filter(listing => {
  //       const tagNames = this.enabledTagNames
  //         .filter(tagName => DetailedListing.hasTag(tagName, listing));
  //
  //       return tagNames.length > 0;
  //     });
  // }
}

export default FilterableController;

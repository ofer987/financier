'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import DatePicker from "react-datepicker";
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
import { Amount } from "./Amount";
import { MonthlyRecord } from "./MonthlyRecord";
import { MonthlyGraph } from "./MonthlyGraph";
import { MonthlyCashFlow, Props, State, CashFlow } from "./MonthlyCashFlow";

// CSS
import "./index.scss";

interface CashFlows {
  getMonthlyProjectedCashFlows: CashFlow[];
}

class PredictionCashFlow extends MonthlyCashFlow {
  public get initialPredictionDate(): Date {
    return new Date(this.predictionYear, this.predictionMonth - 1);
  }

  public get predictionMonth(): number {
    return this.props.predictionMonth;
  }

  public get predictionYear(): number {
    return this.props.predictionYear;
  }

  constructor(props: Props) {
    super(props);

    // this.state = new Array<MonthlyListing>();
    this.setData();
  }

  protected setData(): void {
    // Convert to async/await
    this.client.query<CashFlows>({
      query: gql`
        query {
          getMonthlyProjectedCashFlows(fromYear: ${this.fromYear}, fromMonth: ${this.fromMonth}, toYear: ${this.toYear}, toMonth: ${this.toMonth}, projectedToYear: ${this.predictionYear}, projectedToMonth: ${this.predictionMonth}) {
            isPrediction
            year
            month
            debit
            credit
            profit
          }
        }
      `
    }).then(value => {
      const records = this.toRecords(value.data.getMonthlyProjectedCashFlows);

      this.setState({
        records: records,
        selectedPredictionDate: this.initialPredictionDate
      });
    });
  }
}

export { PredictionCashFlow, Props };

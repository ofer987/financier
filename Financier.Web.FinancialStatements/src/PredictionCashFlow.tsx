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
import { MonthlyCashFlow, Props as MonthlyProps, CashFlow } from "./MonthlyCashFlow";

// CSS
import "./index.scss";

interface Props extends MonthlyProps {
  predictionYear: number;
  predictionMonth: number;
}

interface CashFlows {
  getMonthlyCashFlows: CashFlow[];
}

interface State {
  records: MonthlyRecord[];
  predictionDate: Date;
}

class PredictionCashFlow extends React.Component<Props, State> {
  setPredictionDate(newDate: Date): void {
    this.setState({
      predictionDate: newDate
    });
  }

  get initialPredictionDate(): Date {
    return new Date(this.toYear, this.toMonth);
  }

  get predictionDate(): Date {
    return (this.state || { predictionDate: new Date() }).predictionDate;
  }

  get records(): MonthlyRecord[] {
    return (this.state || { records: [] }).records;
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

  get toDate(): Date {
    return new Date(this.toYear, this.toMonth - 1);
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

    // this.state = new Array<MonthlyListing>();
    this.setData();
  }

  setData(): void {
    // Convert to async/await
    this.client.query<CashFlows>({
      query: gql`
        query {
          getMonthlyCashFlows(fromYear: ${this.fromYear}, fromMonth: ${this.fromMonth}, toYear: ${this.toYear}, toMonth: ${this.toMonth}) {
            year
            month
            debit
            credit
            profit
          }
        }
      `
    }).then(value => {
      const records = this.toRecords(value.data.getMonthlyCashFlows);

      this.setState({
        records: records,
        predictionDate: this.initialPredictionDate
      });
    });
  }

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
          <div className="predictions">
            <DatePicker
              id="prediction-date-picker"
              inline={true}
              selected={this.toDate}
              dateFormat="LLLL/yyyy"
              minDate={new Date(this.toYear, this.toMonth)}
              showMonthYearPicker
              // @ts-ignore
              onChange={(date) => this.setPredictionDate(date)}
            />
          </div>
          <div className={`button monthly-chart`} onClick={(event) => {
            event.preventDefault();

            window.location.pathname = `/prediction-view/from-year/${this.fromYear}/from-month/${this.fromMonth}/to-year/${this.toYear}/to-month/${this.toMonth}/prediction-year/${this.predictionDate.getFullYear()}/prediction-month/${this.predictionDate.getMonth() + 1}`;
          }}>
            View Prediction Chart to {this.toDateString(this.predictionDate)}
          </div>
          <div className="yearly-navigation">
            {lodash.range(this.fromYear, this.toYear + 1, 1).map((year: number) => {
              return this.renderMonthlyNavigation(year);
            })}
          </div>
        </div>
        <div className="monthly-cashflow">
          <MonthlyGraph records={this.records} />
        </div>
        <MonthlyValues records={this.records} />
      </div>
    );
  }

  private toRecords(values: CashFlow[]): Array<MonthlyRecord> {
    const results = values.map(item => {
      return {
        isPrediction: item.isPrediction,
        year: item.year,
        month: item.month - 1,
        amount: new Amount(item.credit, item.debit)
      };

    });

    return results;
  }

  private toDateString(input: Date): string {
    const formatter = d3.timeFormat("%B %Y");

    return formatter(input);
  }
}

export { PredictionCashFlow, Props };

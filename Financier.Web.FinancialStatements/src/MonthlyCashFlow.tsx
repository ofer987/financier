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
import * as Constants from "./auth/components/api-authorization/ApiAuthorizationConstants"

// CSS
import "./index.scss";

interface Props {
  predictionYear: number;
  predictionMonth: number;
  fromYear: number;
  fromMonth: number;
  toYear: number;
  toMonth: number;
}

interface CashFlow {
  isPrediction: boolean;
  year: number;
  month: number;
  debit: number;
  credit: number;
  profit: number;
}

interface CashFlows {
  getMonthlyCashFlows: CashFlow[];
}

interface State {
  records: MonthlyRecord[];
  selectedPredictionDate: Date;
}

class MonthlyCashFlow extends React.Component<Props, State> {
  setSelectedPredictionDate(newDate: Date): void {
    this.setState({
      selectedPredictionDate: newDate
    });
  }

  get initialPredictionDate(): Date {
    return new Date(this.toYear, this.toMonth);
  }

  get selectedPredictionDate(): Date {
    return (this.state || { selectedPredictionDate: new Date() }).selectedPredictionDate;
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

  protected client = new ApolloClient({
    uri: `https://localhost:${Constants.Port}/graphql/cash-flows`,
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

  protected setData(): void {
    // Convert to async/await
    this.client.query<CashFlows>({
      query: gql`
        query {
          getMonthlyCashFlows(fromYear: ${this.fromYear}, fromMonth: ${this.fromMonth}, toYear: ${this.toYear}, toMonth: ${this.toMonth}) {
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
      const records = this.toRecords(value.data.getMonthlyCashFlows);

      this.setState({
        records: records,
        selectedPredictionDate: this.initialPredictionDate
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
              selected={this.selectedPredictionDate}
              dateFormat="LLLL/yyyy"
              minDate={new Date(this.toYear, this.toMonth)}
              showMonthYearPicker
              // @ts-ignore
              onChange={(date) => this.setSelectedPredictionDate(date)}
            />
          </div>
          <div className={`button monthly-chart`} onClick={(event) => {
            event.preventDefault();

            window.location.pathname = `/prediction-view/from-year/${this.fromYear}/from-month/${this.fromMonth}/to-year/${this.toYear}/to-month/${this.toMonth}/prediction-year/${this.selectedPredictionDate.getFullYear()}/prediction-month/${this.selectedPredictionDate.getMonth() + 1}`;
          }}>
            View Prediction Chart to {this.toDateString(this.selectedPredictionDate)}
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

  protected toRecords(values: CashFlow[]): Array<MonthlyRecord> {
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

export { MonthlyCashFlow, Props, State, CashFlow };

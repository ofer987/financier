import React, { useState } from "react";
import DatePicker from "react-datepicker";
import * as d3TimeFormat from "d3-time-format";

import "react-datepicker/dist/react-datepicker.css";

interface Props {}
interface State {
  isValid: boolean;
  startDate: Date;
  endDate: Date;
}

class Welcome extends React.Component<Props, State> {
  get isValid() {
    if (this.endDate < this.startDate) {
      return false;
    }

    return true;
  }

  get isMonthlyChartButtonEnabled() {
    return this.isValid && !this.sameSelectedDates;
  }

  get isDetailedChartButtonEnabled() {
    return this.isValid && this.sameSelectedDates;
  }

  get sameSelectedDates() {
    return true
      && this.startYear == this.endYear
      && this.startMonth == this.endMonth;
  }

  get startYear(): string {
    return d3TimeFormat.timeFormat("%Y")(this.startDate);
  }

  get startMonth(): number {
    return this.startDate.getMonth() + 1;
  }

  get startDate(): Date {
    return this.state.startDate;
  }

  get endYear(): string {
    return d3TimeFormat.timeFormat("%Y")(this.endDate);
  }

  get endMonth(): number {
    return this.endDate.getMonth() + 1;
  }

  get endDate(): Date {
    return this.state.endDate;
  }

  constructor(props: Props) {
    super(props);

    this.state = {
      isValid: true,
      startDate: new Date(),
      endDate: new Date()
    };
  }

  setStartDate(newDate: Date): void {
    this.setState({
      startDate: newDate
    });
  }

  setEndDate(newDate: Date): void {
    this.setState({
      endDate: newDate
    });
  }

  private renderErrors() {
    if (this.isValid) {
      return;
    }

    return (
      <div className="error">
        To Date should be after From Date
      </div>
    );
  }

  private renderSubmission() {
    const monthlyButtonClassName = this.isMonthlyChartButtonEnabled
      ? "enabled"
      : "disabled";
    const detailedButtonClassName = this.isDetailedChartButtonEnabled
      ? "enabled"
      : "disabled";

    return (
      <div className="navigation">
        <div className={`button monthly-chart ${monthlyButtonClassName}`} onClick={(event) => {
          if (!this.isMonthlyChartButtonEnabled) {
            return;
          }

          event.preventDefault();

          window.location.pathname = `/monthly-view/from-year/${this.startYear}/from-month/${this.startMonth}/to-year/${this.endYear}/to-month/${this.endMonth}`;
        }}>
          View Monthly Chart
        </div>
        <div className={`button detailed-chart ${detailedButtonClassName}`} onClick={(event) => {
          if (!this.isDetailedChartButtonEnabled) {
            return;
          }

          event.preventDefault();

          window.location.pathname = `/detailed-view/year/${this.startYear}/month/${this.startMonth}`;
        }}>
          View Detailed Chart
        </div>
      </div>
    );
  }

  render() {
    return (
      <div className="welcome">
        <div className="errors">
          {this.renderErrors()}
        </div>

        <div className="date-range">
          <div className="date-picker start">
            <div className="container">
              <div className="name">From</div>
              <DatePicker
                id="start-date-picker"
                inline={true}
                selected={this.getInitialStartDate()}
                dateFormat="LLLL/yyyy"
                showMonthYearPicker
                // @ts-ignore
                onChange={(date) => this.setStartDate(date)}
              />
            </div>
          </div>
          <div className="date-picker end">
            <div className="container">
              <div className="name">To</div>
              <DatePicker
                id="end-date-picker"
                inline={true}
                selected={this.endDate}
                dateFormat="LLLL/yyyy"
                showMonthYearPicker
                // @ts-ignore
                onChange={(date) => this.setEndDate(date)}
              />
            </div>
          </div>
        </div>

        <div className="submission">
          {this.renderSubmission()}
        </div>
      </div>
    );
  }

  private getInitialStartDate(): Date {
    let date = new Date();
    let year = date.getFullYear() - 5;
    let month = date.getMonth();
    console.log(`${year}: ${month}`);

    return new Date(year, month);
  }
}

export default Welcome;

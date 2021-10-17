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
    return (parseInt(`${this.endYear}${this.endMonth}`) >= parseInt(`${this.startYear}${this.startMonth}`));
  }

  get startYear(): string {
    return d3TimeFormat.timeFormat("%Y")(this.startDate);
  }

  get startMonth(): string {
    return d3TimeFormat.timeFormat("%m")(this.startDate);
  }

  get startDate(): Date {
    return this.state.startDate;
  }

  get endYear(): string {
    return d3TimeFormat.timeFormat("%Y")(this.endDate);
  }

  get endMonth(): string {
    return d3TimeFormat.timeFormat("%m")(this.endDate);
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
    console.log(`changing date: ${newDate}`);
    this.setState({
      startDate: newDate
    });
  }

  setEndDate(newDate: Date): void {
    console.log(`changing date: ${newDate}`);
    this.setState({
      endDate: newDate
    });
  }

  private renderErrors() {
    if (this.isValid) {
      return (
        <div></div> 
      );
    }

    return (
      <div>
        To Date should be after From Date
      </div>
    );
  }

  private renderSubmission() {
    if (this.isValid) {
      console.log("is valid");
      return (
        <a href={`/monthly-view/year/${this.startYear}`}>View</a>
      );
    }

    return (
      <div></div>
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
                selected={this.startDate}
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
}

export default Welcome;

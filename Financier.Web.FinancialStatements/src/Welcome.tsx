import React, { useState } from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

interface Props {}
interface State {
  date: Date;
}

class MonthPicker extends React.Component<Props, State> {
  get date(): Date {
    return this.state.date;
  }

  constructor(props: Props) {
    super(props);

    this.state = {
      date: new Date()
    };

    // const [startDate, setStartDate] = React.useState(new Date());
  }

  setDate(newDate: Date): void {
    console.log(`changing date: ${newDate}`);
    this.setState({
      date: newDate
    });
  }

  render() {
    return (
      <div className="date-range">
        <div className="date-picker start">
          <div className="name">From</div>
          <DatePicker 
            id="start-date-picker"
            selected={this.date} 
            dateFormat="LLLL/yyyy" 
            showMonthYearPicker 
            // @ts-ignore 
            onChange={(date) => this.setDate(date)} 
          />
        </div>
        <div className="date-picker end">
          <div className="name">To</div>
          <DatePicker 
            id="end-date-picker"
            selected={this.date} 
            dateFormat="LLLL/yyyy" 
            showMonthYearPicker 
            // @ts-ignore 
            onChange={(date) => this.setDate(date)} 
          />
        </div>
      </div>
    );
  }
}

export default MonthPicker;

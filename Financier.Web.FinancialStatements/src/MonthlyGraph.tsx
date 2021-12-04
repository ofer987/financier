import * as React from "react";
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3";
import * as d3Shape from "d3-shape";
import * as d3Scale from "d3-scale";
import * as d3Format from "d3-format";
import * as d3TimeFormat from "d3-time-format";

import { Listing, ExpenseTypes } from "./Listing";

interface MonthlyProps {
  dates: MonthlyProp[];
}

interface MonthlyProp {
  year: number;
  month: number;
  listing: Listing;
  // credit: Listing;
  // debit: Listing;
}

interface Value {
  date: Date;
  value: number;
}

class MonthlyGraph extends React.Component<MonthlyProps> {
  width = 500;
  height = 300;

  margin = {
    top: 10,
    right: 100,
    bottom: 20,
    left: 40,
  }

  get cumulativeProfits(): Value[] {
    let accumulator = 0;

    return this.profits.map(profit => {
      accumulator += profit.value;

      return {
        date: profit.date,
        value: accumulator
      };
    });
  }

  get profits(): Value[] {
    return this.props.dates.map(item => {
      return {
        date: item.at,
        value: item.listing.profitAmount
      };
    });
  }

  get credits(): Value[] {
    return this.props.dates.map(item => {
      return {
        date: item.at,
        value: item.listing.creditAmount
      };
    });
  }

  get debits(): Value[] {
    return this.props.dates.map(item => {
      return {
        date: item.at,
        value: item.listing.debitAmount
      };
    });
  }

  componentDidUpdate() {
    const data = this.props;

    // Remove existing chart elements (if exist)
    document.querySelectorAll(".graph .chart g").forEach(node => node.remove());

    // Recreate chart elements
    if (this.props.dates.length == 0) {
      return;
    }

    this.drawViewBox();
    this.drawXAxis();
    this.drawYAxis();
    this.drawChart(this.credits, "credits", "black");
    this.drawChart(this.debits, "debits", "red");
    this.drawChart(this.profits, "profits", "blue");
    this.drawChart(this.cumulativeProfits, "cumulativeProfits", "darkblue");

    this.drawLegend(["credits", "debits"]);
  }

  render() {
    return (
      <div className="graph">
        <h2>Graph</h2>
        <svg className="chart" />
      </div>
    );
  }

  private get highestValue(): number {
    let values = this.credits
      .concat(this.debits)
      .concat(this.profits)
      .concat(this.cumulativeProfits)

    return d3.max(values.map(d => d.value), d => d);
  }

  private get lowestValue(): number {
    return d3.min(this.profits.map(d => d.value), d => d);
  }

  private xScale() {
    return d3.scaleTime()
      .domain(d3.extent(this.props.dates, d => d.at))
      .range([this.margin.left, this.width - this.margin.right]);
  }

  private yScale() {
    return d3.scaleLinear()
      .domain([this.lowestValue, this.highestValue])
      .nice(5)
      .range([this.height - this.margin.bottom, this.margin.top]);
  }

  private myLine() {
    return d3Shape.line()
      .x(d => this.xScale()(d["date"]))
      .y(d => this.yScale()(d["value"]));
  }

  private drawXAxis() {
    const svg = d3.select("svg.chart");

    svg.append("g")
      .attr("class", "x-axis")
      .attr("transform", `translate(0,${this.height - this.margin.bottom})`)
      .call(d3.axisBottom(this.xScale()).ticks(this.width / 40).tickSizeOuter(0));
  }

  private drawYAxis() {
    const svg = d3.select("svg.chart");

    svg.append("g")
      .attr("class", "y-axis")
      .attr("transform", `translate(${this.margin.left}, 0)`)
      .call(d3.axisLeft(this.yScale()))
      .call(g => g.select(".domain"))
      .call(g => g.select(".tick:last-of-type text").clone()
        .attr("y", "-2em")
        .attr("text-anchor", "start")
        .attr("font-weight", "bold")
        .attr("class", "label")
        .text("Amount ($)")
      );
  }

  private drawLegend(names: string[]) {
    const svg = d3.select("svg.chart");
    svg.attr("viewBox", `0, 0, ${this.width}, ${this.height}`);

    let box = svg.append("rect")
      .attr("y", 0)
      .attr("x", 0)
      .attr("rx", 0)
      .attr("ry", 0)
      .selectAll("g")
      .data(names)
      .append("title")
        .text(text => names[0]);
  }

  private drawViewBox() {
    const svg = d3.select("svg.chart");

    svg.attr("viewBox", `0, 0, ${this.width}, ${this.height}`);
  }

  private drawChart(values: Value[], name: string, colour: string) {
    const svg = d3.select("svg.chart");

    let path = svg.append("path")
      .datum(values)
      .attr("fill", "none")
      .attr("stroke", colour)
      .attr("stroke-width", 1.5)
      .attr("stroke-linejoin", "round")
      .attr("stroke-linecap", "round")
      // @ts-ignore
      .attr("d", this.myLine());
      // .attr("text", name)
      // .call(text => text.append("tspan")
      //   .attr("x", -6)
      //   .attr("y", "1.15em"));

    let y = this.yScale()(values[values.length - 1].value)
    svg.append("text")
      .datum(values)
      .text(lodash.startCase(name))
      .attr("class", "label")
      // .attr("font-size", "0.375em")
      .attr("x", this.width - this.margin.right)
      .attr("dx", "0.25em")
      .attr("y", y)
      .attr("dy", "0.25em");

    // svg.append("g")
    //   .append("text")
    //   .append(name);

    // @ts-ignore
    // alert(_.startCase(name));
    // svg.append("text")
    //   // @ts-ignore
    //   .attr("text", _.startCase(name));
  }

  private getName(at: Date): string {
    const year = at.getFullYear();
    const month = d3TimeFormat.timeFormat("%B")(at);

    return `${year} - ${month}`;
  }
};

export { MonthlyGraph, MonthlyProps, MonthlyProp };

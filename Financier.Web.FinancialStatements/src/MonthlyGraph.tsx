import * as React from "react";
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3";
import * as d3Shape from "d3-shape";
import * as d3Scale from "d3-scale";
import * as d3Format from "d3-format";
import * as d3TimeFormat from "d3-time-format";

import { Listing, ExpenseTypes } from "./Listing";
import CashFlowModel from "./CashFlowModel";
import FilterableController from "./FilterableController";

interface MonthlyProps {
  dates: MonthlyProp[];
}

interface MonthlyProp {
  at: Date;
  credit: Listing;
  debit: Listing;
}

interface Value {
  date: Date;
  value: number;
}

class MonthlyGraph extends React.Component<MonthlyProps> {
  width = 400;
  height = 400;

  margin = {
    top: 30,
    right: 0,
    bottom: 100,
    left: 40,
  }

  private get credits(): Value[] {
    return this.props.dates.map(item => {
      return {
        date: item.at,
        value: item.credit.amount
      };
    });
  }

  componentDidUpdate() {
    const data = this.props;

    // Remove existing chart elements (if exist)
    // document.querySelectorAll(".graph .chart g").forEach(node => node.remove());

    // Recreate chart elements
    this.chart();
  }

  render() {
    return (
      <div className="graph">
        <h2>Graph</h2>
        <svg className="chart" />
      </div>
    );
  }

  private xScale() {
    return d3.scaleTime()
      .domain(d3.extent(this.credits, d => d.date))
      .range([this.margin.left, this.width - this.margin.right]);
  }

  private yScale() {
    return d3.scaleLinear()
      .domain([0, d3.max(this.credits.map(d => d.value), d => d)])
      .nice(5)
      .range([this.height - this.margin.bottom, this.margin.top]);
  }

  // private line(data: MonthlyProp[]) {
  //   return d3.line((d, index, ds) => index, (d, index, ds) => index);
  // }

  private myLine() {
    return d3Shape.line()
      .x(d => this.xScale()(d["date"]))
      .y(d => this.yScale()(d["value"]));
  }

  private chart() {
    const data = this.credits;

    if (data.length == 0) {
      return;
    }

    const svg = d3.select("svg.chart");
    svg.attr("viewBox", `0, 0, ${this.width}, ${this.height}`);

    const gaga = d3Shape.line()
      .x(d => this.xScale()(d["date"]))
      .y(d => this.yScale()(d["value"]));

    svg.append("path")
      .datum(data)
      .attr("fill", "none")
      .attr("stroke", "steelblue")
      .attr("stroke-width", 1.5)
      .attr("stroke-linejoin", "round")
      .attr("stroke-linecap", "round")
      // @ts-ignore
      .attr("d", gaga)

    svg.append("g")
      .attr("class", "y-axis")
      .attr("transform", `translate(${this.margin.left},0)`)
      .call(d3.axisLeft(this.yScale()))
      .call(g => g.select(".domain").remove())
      .call(g => g.select(".tick:last-of-type text").clone()
        .attr("x", 3)
        .attr("text-anchor", "start")
        .attr("font-weight", "bold")
        .text("Amount ($)")
      );

    svg.append("g")
      .attr("class", "x-axis")
      .attr("transform", `translate(0,${this.height - this.margin.bottom})`)
      .call(d3.axisBottom(this.xScale()).ticks(this.width / 80).tickSizeOuter(0));
  }

  private getName(at: Date): string {
    const year = at.getFullYear();
    const month = d3TimeFormat.timeFormat("%B")(at);

    return `${year} - ${month}`;
  }

  private colour(expenseType: ExpenseTypes) {
    switch (expenseType) {
      case ExpenseTypes.Credit:
      return "black";
      case ExpenseTypes.Debit:
      return "red";
    }
  }

  private createUniqueKey(at: Date): string {
    return `${at.getFullYear()}-${at.getMonth() + 1}`;
  }
};

export { MonthlyGraph, MonthlyProps, MonthlyProp };

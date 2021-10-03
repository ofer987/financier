import * as React from "react";
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3";
import * as d3Scale from "d3-scale";
import * as d3Format from "d3-format";
import * as d3TimeFormat from "d3-time-format";

import { Listing, ExpenseTypes } from "./Listing";
import CashFlowModel from "./CashFlowModel";
import FilterableController from "./FilterableController";

interface Props {
  dates: Prop[];
}

interface Prop {
  at: Date;
  credit: Listing;
  debit: Listing;
}

class MonthlyGraph extends React.Component<Props> {
  width = 400;
  height = 400;

  margin = {
    top: 30,
    right: 0,
    bottom: 100,
    left: 40,
  }

  componentDidUpdate() {
    const data = this.props;

    // Remove existing chart elements (if exist)
    document.querySelectorAll(".graph .chart g").forEach(node => node.remove());

    // Recreate chart elements
    this.chart(data.dates);
  }

  render() {
    return (
      <div className="graph">
        <svg className="chart" />
      </div>
    );
  }

  private xScale(data: Prop[]) {
    return d3.scaleUtc()
      .domain(d3.extent(data, d => d.at))
      .range([this.margin.left, this.width - this.margin.right]);
      // .padding(0.1);
  }

  private yScale(data: Prop[]) {
    return d3.scaleLinear()
      .domain([0, d3.max(data.map(d => d.credit.amount), d => d)])
      .nice(5)
      .range([this.height - this.margin.bottom, this.margin.top]);
  }

  private line(data: Prop[]) {
    return d3.line((d, index, ds) => index, (d, index, ds) => index);
  }

  private chart(values: Prop[]) {
    const data = values;

    if (data.length == 0) {
      return;
    }

    const svg = d3.select("svg.chart");
    svg.attr("viewBox", `0, 0, ${this.width}, ${this.height}`);

    // TODO: Convert to a Line Chart
    // https://observablehq.com/@d3/line-chart
    svg.append("path")
      .datum(data)
      .attr("fill", "none")
      .attr("stroke", "steelblue")
      .attr("stroke-width", 1.5)
      .attr("stroke-linejoin", "round")
      .attr("stroke-linecap", "round")
      .attr("d", (d) => this.line(d[0]));
      // .attr("x", (d, i) => this.xScale(data)(this.getName(d)))
      // .attr("y", d => this.yScale(data)(d.amount))
      // .attr("height", d => this.yScale(data)(0) - this.yScale(data)(d.amount))
      // .attr("width", this.xScale(data).bandwidth());

    svg.append("g")
      .attr("class", "y-axis")
      .attr("transform", `translate(${this.margin.left},0)`)
      .call(d3.axisLeft(this.yScale(data)))
      .call(g => g.select(".domain").remove())
      .call(g => g.select(".tick:last-of-type text").clone()
        .attr("x", 3)
        .attr("text-anchor", "start")
        .attr("font-weight", "bold")
        .text((_element, d) => d)
      );

    svg.append("g")
      .attr("class", "x-axis")
      .attr("transform", `translate(0,${this.height - this.margin.bottom})`)
      .call(d3.axisBottom(this.xScale(data)).ticks(this.width / 80).tickSizeOuter(0));
      // .selectAll("text")
      // .attr("x", "0")
      // .attr("y", "2")
      // .attr("dx", "-10px")
      // .attr("dy", "0")
      // .attr("transform", "rotate(-90, 0, 0)");
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

export { MonthlyGraph };

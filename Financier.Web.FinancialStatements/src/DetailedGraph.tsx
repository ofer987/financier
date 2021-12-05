import * as React from "react";
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3";
import * as d3Scale from "d3-scale";
import * as d3Format from "d3-format";

import { Listing } from "./Listing";
import { DetailedListing } from "./DetailedCashFlow";

class DetailedGraph extends React.Component<DetailedListing[]> {
  width = 500;
  height = 500;

  margin = {
    top: 10,
    right: 0,
    bottom: 100,
    left: 40,
  };

  componentDidUpdate() {
    const data = this.enabledCredits.concat(this.enabledDebits);

    // Remove existing chart elements (if exist)
    document.querySelectorAll(".graph .chart g").forEach(node => node.remove());

    // Recreate chart elements
    this.chart(data);
  }

  render() {
    return (
      <div className="graph">
        <h2>Graph</h2>
        <svg className="chart" />
      </div>
    );
  }

  private xScale(data: Listing[]) {
    return d3.scaleBand()
      .domain(data.map(item => item.tags.join(", ")))
      .range([this.margin.left, this.width - this.margin.right])
      .padding(0.1);
  }

  private yScale(data: Listing[]) {
    return d3.scaleLinear()
      .domain([0, d3.max(data, d =>  d.amount)])
      // .nice(5)
      .range([this.height - this.margin.bottom, this.margin.top]);
  }

  private chart(values: Listing[]) {
    const data = values;

    if (data.length == 0) {
      return;
    }

    const svg = d3.select("svg.chart");
    svg.attr("viewBox", `0, 0, ${this.width}, ${this.height}`);

    let bar = svg.append("g")
      .selectAll("rect")
      .data(data)

    bar.join("rect")
      .attr("fill", (d) => this.colour(d.expenseType))
      .attr("x", (d, i) => this.xScale(data)(d.tags.join(", ")))
      .attr("y", d => this.yScale(data)(d.amount))
      .attr("height", d => this.yScale(data)(0) - this.yScale(data)(d.amount))
      .attr("width", this.xScale(data).bandwidth());

    bar.join("text")
      .text(d => d.amount)
      .attr("x", d => this.xScale(data)(d.tags.join(", ")) + this.xScale(data).bandwidth() / 2)
      .attr("y", d => this.yScale(data)(d.amount) - 2)
      .attr("font-size", "6px")
      .attr("text-anchor", "middle")
      .attr("dx", )

    svg.append("g")
      .attr("class", "y-axis")
      .attr("transform", `translate(${this.margin.left}, 0)`)
      // .call(d3.axisLeft(this.yScale(data)).tickFormat(d3Format.format("5")))
      .call(d3.axisLeft(this.yScale(data)))
      .call(g => g.select(".domain"))
      .call(g => g.select(".tick:last-of-type text").clone()
        .attr("y", "-2em")
        .attr("dy", "-0.25em")
        .attr("text-anchor", "start")
        .attr("font-weight", "bold")
        .attr("class", "label")
        .text("Amount ($)")
      );
      // .call(g => g.select(".tick:last-of-type text").clone());
      // .call(g => g.select(".tick:last-of-type text").clone()
      //   .attr("y", "-2em")
      //   .attr("text-anchor", "start")
      //   .attr("font-weight", "bold")
      //   .attr("class", "label")
      //   .text("Amount ($)")
      // );

    svg.append("g")
      .attr("class", "x-axis")
      .attr("transform", `translate(0,${this.height - this.margin.bottom})`)
      .call(d3.axisBottom(this.xScale(data)).tickFormat(i => i).tickSizeOuter(0))
      .selectAll("text")
      .attr("x", "0")
      .attr("y", "2")
      .attr("dx", "-10px")
      .attr("dy", "0")
      .attr("transform", "rotate(-90, 0, 0)");
  }

  private colour(expenseType: ExpenseTypes) {
    switch (expenseType) {
      case ExpenseTypes.Credit:
      return "black";
      case ExpenseTypes.Debit:
      return "red";
    }
  }

  private createUniqueKey(item: DetailedListing): string {
    return item.tags.join("-");
  }
};

export { DetailedGraph };

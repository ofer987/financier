import * as React from "react";
import _ from "underscore";
import * as d3 from "d3";
import * as d3Scale from "d3-scale";
import * as d3Format from "d3-format";

import { Listing, ExpenseTypes } from "./Listing";
import CashFlowModel from "./CashFlowModel";

interface Props {
  debits: Listing[];
  credits: Listing[];
}

class Graph extends React.Component<Props> {
  constructor(props) {
    super(props);
  }

  componentDidUpdate() {
    const data = this.props.credits.concat(this.props.debits);
    this.chart(data);
  }

  debits: Listing[];
  width = 400;
  height = 400;

  margin = {
    top: 30,
    right: 0,
    bottom: 200,
    left: 40,
  }

  allTags() {
    var creditTags = this.props.credits.map(item => item.tags);
    var debitTags = this.props.debits.map(item => item.tags);

    var tags = creditTags.concat(debitTags);

    return _.uniq(tags);
  }

  xScale(data: Listing[]) {
    return d3.scaleBand()
      .domain(data.map(item => item.tags.join(", ")))
      .range([this.margin.left, this.width - this.margin.right])
      .padding(0.1);
  }

  yScale(data: Listing[]) {
    return d3.scaleLinear()
      .domain([0, d3.max(data, d =>  d.amount)])
      .nice(5)
      .range([this.height - this.margin.bottom, this.margin.top]);
  }

  chart(values: Listing[]) {
    const data = values;

    if (data.length == 0) {
      return;
    }

    const svg = d3.select("svg.chart");
    svg.attr("viewBox", `0, 0, ${this.width}, ${this.height}`);

    svg.append("g")
      .selectAll("rect")
      .data(data)
      .join("rect")
      .attr("fill", (d) => this.colour(d.expenseType))
      .attr("x", (d, i) => this.xScale(data)(d.tags.join(", ")))
      .attr("y", d => this.yScale(data)(d.amount))
      .attr("height", d => this.yScale(data)(0) - this.yScale(data)(d.amount))
      .attr("width", this.xScale(data).bandwidth());

    svg.append("g")
      .attr("class", "y-axis")
      .attr("transform", `translate(${this.margin.left},0)`)
      .call(d3.axisLeft(this.yScale(data)).tickFormat(d3Format.format("5")))

    svg.append("g")
      .attr("class", "x-axis")
      .attr("transform", `translate(0,${this.height - this.margin.bottom})`)
      .call(d3.axisBottom(this.xScale(data)).tickFormat(i => i).tickSizeOuter(0))
      .selectAll("text")
      .attr("x", "0")
      .attr("y", "0")
      .attr("dx", "-10px")
      .attr("dy", "0")
      .attr("transform", "rotate(-90, 0, 0)");
  }

  colour(expenseType: ExpenseTypes) {
    switch (expenseType) {
      case ExpenseTypes.Credit:
      return "black";
      case ExpenseTypes.Debit:
      return "red";
    }
  }

  render() {
    return (
      <div className="graph">
        <svg className="chart" />
      </div>
    );
  }

  private createUniqueKey(item: CashFlowModel): string {
    return item.tags.join("-");
  }
};

export { Graph };

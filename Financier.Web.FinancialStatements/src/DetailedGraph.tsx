import * as React from "react";
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3";
import * as d3Scale from "d3-scale";
import * as d3Format from "d3-format";

import { DetailedRecord } from "./DetailedRecord";

interface Props {
  records: DetailedRecord[];
}

class DetailedGraph extends React.Component<Props> {
  width = 500;
  height = 500;

  margin = {
    top: 10,
    right: 0,
    bottom: 100,
    left: 40,
  };

  componentDidUpdate() {
    // Remove existing chart elements (if exist)
    document.querySelectorAll(".graph .chart g").forEach(node => node.remove());

    // Recreate chart elements
    this.chart(this.props.records);
  }

  render() {
    return (
      <div className="graph">
        <h2>Graph</h2>
        <svg className="chart" />
      </div>
    );
  }

  private xScale(data: DetailedRecord[]) {
    return d3.scaleBand()
      .domain(data.map(item => item.tags.join(", ")))
      .range([this.margin.left, this.width - this.margin.right])
      .padding(0.1);
  }

  private yScale(data: DetailedRecord[]) {
    return d3.scaleLinear()
      .domain([0, d3.max(data, d =>  d.amount.profit)])
      // .nice(5)
      .range([this.height - this.margin.bottom, this.margin.top]);
  }

  private chart(values: DetailedRecord[]) {
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
      .attr("fill", (d) => this.colour(d.amount.profit))
      .attr("x", (d, i) => this.xScale(data)(d.tags.join(", ")))
      .attr("y", d => this.yScale(data)(d.amount.profit))
      .attr("height", d => this.yScale(data)(0) - this.yScale(data)(d.amount.profit))
      .attr("width", this.xScale(data).bandwidth());

    bar.join("text")
      .text(d => d.amount.profit)
      .attr("x", d => this.xScale(data)(d.tags.join(", ")) + this.xScale(data).bandwidth() / 2)
      .attr("y", d => this.yScale(data)(d.amount.profit) - 2)
      .attr("font-size", "6px")
      .attr("text-anchor", "middle")
      .attr("dx", )

    svg.append("g")
      .attr("class", "y-axis")
      .attr("transform", `translate(${this.margin.left}, 0)`)
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

  private colour(profit: number) {
    if (profit >=0) {
      return "black";
    }

    return "red";
  }

  private createUniqueKey(item: DetailedRecord): string {
    return item.tags.join("-");
  }
};

export { DetailedGraph };

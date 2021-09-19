import * as React from "react";
import * as d3 from "d3";
import * as d3Scale from "d3-scale";

import Listing from "./Listing";
import CashFlowModel from "./CashFlowModel";

interface Props {
  debits: Listing[];
  credits: Listing[];
}

class State {
}

class Graph extends React.Component<Props, State> {
  constructor(props) {
    super(props);

    // this.state = new State(this.props.debits);
    // this.state.chart();
  }

  componentDidMount() {
    // this.setState(new State(this.props.debits));
  }

  componentDidUpdate() {
    this.chart(this.props.debits, "red");
    this.chart(this.props.credits, "black");
  }

  debits: Listing[];
  width = 400;
  height = 400;

  margin = {
    top: 30,
    right: 0,
    bottom: 30,
    left: 40,
  }

  // Get the debits
  data(values: Listing[]): { name: string; value: number}[] {
    return values
      .filter(listing => !listing.isNull)
      .map(listing => {
          return {
            name: listing.tags.join(", "),
            value: listing.amount
          }
      });
  }

  format(): string {
    return "$";
  }

  xAxisName(): string {
    return "Tags";
  }

  yAxisName(): string {
    return "Amount";
  }

  x(data) {
    return d3.scaleBand()
      .domain(d3.range(data.length))
      .range([this.margin.left, this.width - this.margin.right])
      .padding(0.1);
  }

  y(data) {
    var result = d3.scaleLinear()
      .domain([0, d3.max(data, d => { return d.value; })]);

    var var2 = result.nice();
    return var2.range([this.height - this.margin.bottom, this.margin.top]);
  }

  xAxis(data) {
    return (g) => g
      .attr("transform", `translate(0,${this.height - this.margin.bottom})`)
      .call(d3.axisBottom(this.x).tickFormat(i => data[i].name).tickSizeOuter(0));
  }

  yAxis(data) {
    return (g) => g
      .attr("transform", `translate(${this.margin.left},0)`)
      .call(d3.axisLeft(this.y).ticks(null, this.format()))
      .call(g => g.select(".domain").remove())
      .call(g => g.append("text")
        .attr("x", -this.margin.left)
        .attr("y", 10)
        .attr("fill", "currentColor")
        .attr("text-anchor", "start")
        .text(this.yAxisName()));
  }

  chart(values: Listing[], colour: string) {
    var height = 400;
    var width = 400;

    var data = this.data(values);

    if (data.length == 0) {
      return;
    }

    const svg = d3.select("svg.chart#debits");
    // const svgOld = (document.querySelector("svg.chart#debits") as SVGElement); // d3.create("svg")
    svg.attr("viewBox", [0, 0, width, height]);

    svg.append("g")
      .attr("fill", colour)
      .selectAll("rect")
      .data(data)
      .join("rect")
      .attr("x", (d, i) => this.x(data)(i))
      .attr("y", d => this.y(data)(d.value))
      .attr("height", d => this.y(data)(0) - this.y(data)(d.value))
      .attr("width", this.x(data).bandwidth());

    svg.append("g")
      .call(() => this.xAxis);

    svg.append("g")
      .call(() => this.yAxis);

    // return svg.node();
  }

  render() {
    return (
      <div>
        <h2>This is a graph</h2>
        <svg className="chart" id="debits" />
      </div>
    );
  }

  private createUniqueKey(item: CashFlowModel): string {
    return item.tags.join("-");
  }
};

export { Graph };

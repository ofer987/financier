import * as React from "react";
import _ from "underscore";
import lodash from "lodash";
import * as d3 from "d3";
import * as d3Shape from "d3-shape";
import * as d3Scale from "d3-scale";
import * as d3Format from "d3-format";
import * as d3TimeFormat from "d3-time-format";

import { Amount } from "./Amount";
import { MonthlyRecord } from "./MonthlyRecord";

interface Props {
  records: MonthlyRecord[];
}

interface Value {
  date: Date;
  value: number;
  isPrediction: boolean;
}

class MonthlyGraph extends React.Component<Props> {
  width = 600;
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
        value: accumulator,
        isPrediction: profit.isPrediction
      };
    });
  }

  get profits(): Value[] {
    return this.props.records.map(item => {
      return {
        date: new Date(item.year, item.month, 1),
        value: item.amount.profit,
        isPrediction: item.isPrediction
      };
    });
  }

  get credits(): Value[] {
    return this.props.records.map(item => {
      return {
        date: new Date(item.year, item.month, 1),
        value: item.amount.credit,
        isPrediction: item.isPrediction
      };
    });
  }

  get debits(): Value[] {
    return this.props.records.map(item => {
      return {
        date: new Date(item.year, item.month, 1),
        value: item.amount.debit,
        isPrediction: item.isPrediction
      };
    });
  }

  componentDidUpdate() {
    const data = this.props;

    // Remove existing chart elements (if exist)
    document.querySelectorAll(".graph .chart g").forEach(node => node.remove());

    // Recreate chart elements
    if (this.props.records.length == 0) {
      return;
    }

    this.drawViewBox();
    this.drawXAxis();
    this.drawYAxis();

    this.drawChart(this.credits, "credits", "rgba(0, 0, 0, 1)", "rgba(0, 0, 0, 0.2)");
    this.drawChart(this.debits, "debits", "rgba(255, 0, 0, 1)", "rgba(255, 0, 0, 0.2)");
    this.drawChart(this.profits, "profits", "rgba(0, 0, 255, 1)", "rgba(0, 0, 255, 0.2)");
    this.drawChart(this.cumulativeProfits, "cumulativeProfits", "rgba(0, 255, 0, 1)", "rgba(0, 255, 0, 0.2)");

    this.drawLegend(["credits", "debits"]);

    document.querySelectorAll(".chart path").forEach(path => {
      let id = path.id;

      path.addEventListener("mouseover", (_event: Event) => {
        document.querySelectorAll(`.chart > path`)
          .forEach(element => {
            if (element.id != id) {
              element.classList.add("hidden");
            }
          });

        document.querySelectorAll(`.chart > text`)
          .forEach(element => {
            if (element.id != id) {
              element.classList.add("hidden");
            }
          });
      });

      path.addEventListener("mouseout", (_event: Event) => {
        document.querySelectorAll(`.chart > path`)
          .forEach(element => {
            if (element.id != id) {
              element.classList.remove("hidden");
            }
          });

        document.querySelectorAll(`.chart > text`)
          .forEach(element => {
            if (element.id != id) {
              element.classList.remove("hidden");
            }
          });
      });
    });
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
    const dates = this.props.records.map(item => {
      return {
        value: 0, // Only the date is important
        date: new Date(item.year, item.month, 1)
      }
    });
    return d3.scaleTime()
      .domain(d3.extent(dates, d => d.date))
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
      .call(d3.axisBottom(this.xScale())
        .tickFormat(d3.timeFormat("%b %Y"))
        .ticks(this.width / 60)
        .tickSizeOuter(0)
      );
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

    svg.append("rect")
      .attr("y", 0)
      .attr("x", 0)
      .attr("rx", 0)
      .attr("ry", 0)
      .selectAll("g")
      .data(names)
      .append("title")
        .text(_text => names[0]);
  }

  private drawViewBox() {
    const svg = d3.select("svg.chart");

    svg.attr("viewBox", `0, 0, ${this.width}, ${this.height}`);
  }

  private drawChart(values: Value[], name: string, colourOne: string, colourTwo: string) {
    const svg = d3.select("svg.chart");

    svg.append("path")
      .datum(values.filter(item => !item.isPrediction))
      .attr("fill", "none")
      .attr("stroke", colourOne)
      .attr("stroke-width", 1.5)
      .attr("stroke-linejoin", "round")
      .attr("stroke-linecap", "round")
      .attr("id", name)
      // @ts-ignore
      .attr("d", this.myLine());
      // .attr("text", name)
      // .call(text => text.append("tspan")
      //   .attr("x", -6)
      //   .attr("y", "1.15em"));

    let lastExistingValue = lodash.last(values.filter(item => !item.isPrediction))
    let predictedValues = [lastExistingValue];

    values
      .filter(item => item.isPrediction)
      .forEach(item => predictedValues.push(item));;

    svg.append("path")
      .datum(predictedValues)
      .attr("fill", "none")
      .attr("stroke", colourTwo)
      .attr("stroke-width", 1.5)
      .attr("stroke-linejoin", "round")
      .attr("stroke-linecap", "round")
      .attr("id", name)
      // @ts-ignore
      .attr("d", this.myLine());

    let y = this.yScale()(values[values.length - 1].value)
    svg.append("text")
      .datum(values)
      .text(lodash.startCase(name))
      .attr("id", name)
      .attr("class", "label")
      .attr("x", this.width - this.margin.right)
      .attr("dx", "0.25em")
      .attr("y", y)
      .attr("dy", "0.25em");
  }
};

export { MonthlyGraph };

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch } from "react-router-dom";

import CashFlow from "./CashFlow";
import MonthlyCashFlow from "./MonthlyCashFlow";

// CSS
import "./index.scss";

function useQuery(): URLSearchParams {
  return new URLSearchParams(useLocation().search);
}

function App() {
  // const { year } = useParams();
  // if (typeof (year) == "undefined") {
  //   console.error("year is not defined");
  // } else {
  //   console.log(year);
  // }
  let query = useQuery();
  const year = parseInt(query.get("year"));
  console.log(year);
  const month = parseInt(query.get("month"));

  return (
    <div>
      <Switch>
        <Route exact path="/detailed-view">
          <CashFlow year={year} month={month} />
        </Route>
        <Route exact path="/monthly-view">
          <MonthlyCashFlow year={year} />
        </Route>
        <Route path="/">
          <MonthlyCashFlow year={2020} />
        </Route>
      </Switch>
    </div>
  );
}

export default App;

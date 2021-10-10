import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";

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
  // let query = useQuery();
  // const year = parseInt(query.get("year"));
  // console.log(year);
  // const month = parseInt(query.get("month"));

  return (
    <Router>
      <h2>Years</h2>
      <Link to="/monthly-view/year/2019">2019&nbsp;</Link>
      <Link to="/monthly-view/year/2020">2020&nbsp;</Link>
      <Link to="/detailed-view/year/2020/month/3">2020-3&nbsp;</Link>
      <Link to="/detailed-view/year/2020/month/4">2020-4&nbsp;</Link>

      <Switch>
        <Route exact path="/detailed-view/year/:year/month/:month" component={CashFlow}>
        </Route>
        <Route exact path="/monthly-view/year/:year" component={MonthlyCashFlow} />
        <Route path="/">
          <MonthlyCashFlow match={ { params: { year: 2020 } } } />
        </Route>
      </Switch>
    </Router>
  );
}

export default App;

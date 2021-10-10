import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";

import CashFlow from "./CashFlow";
import MonthlyCashFlow from "./MonthlyCashFlow";

function App() {
  return (
    <Router>
      <h2>Years</h2>
      <a href="/monthly-view/year/2019">2019&nbsp;</a>
      <a href="/monthly-view/year/2020">2020&nbsp;</a>
      <a href="/detailed-view/year/2020/month/3">2020-3&nbsp;</a>
      <a href="/detailed-view/year/2020/month/4">2020-4&nbsp;</a>

      <Switch>
        <Route path="/detailed-view/year/:year/month/:month" component={CashFlow}>
        </Route>
        <Route path="/monthly-view/year/:year" component={MonthlyCashFlow} />
        <Route path="/">
          <MonthlyCashFlow match={ { params: { year: 2020 } } } />
        </Route>
      </Switch>
    </Router>
  );
}

export default App;

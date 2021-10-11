import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";

import Welcome from "./Welcome";
import MonthlyRoute from "./MonthlyRoute";
import DetailedRoute from "./DetailedRoute";

function App() {
  return (
    <div>
      <h1 className="header">Financier</h1>
      <Router>
        <Switch>
          <Route path="/detailed-view/year/:year/month/:month" component={DetailedRoute}>
          </Route>
          <Route path="/monthly-view/year/:year" component={MonthlyRoute} />
          <Route path="/">
            <Welcome />
          </Route>
        </Switch>
      </Router>
    </div>
  );
}

export default App;

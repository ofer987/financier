import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";

import './auth/setupProxy.js';

import AuthorizeRoute from "./auth/components/api-authorization/AuthorizeRoute";
import Welcome from "./Welcome";
import WelcomeError from "./WelcomeError";
import MonthlyRoute from "./MonthlyRoute";
import PredictionRoute from "./PredictionRoute";
import DetailedRoute from "./DetailedRoute";
import ItemizedRoute from "./ItemizedRoute";
import AllItemsRoute from './AllItemsRoute';

function App() {
  return (
    <div>
      <h1 className="main-header">Financier</h1>
      <Router>
        <Switch>
          <Route path="/itemized-view/year/:year/month/:month/tagNames/:tags" component={ItemizedRoute}>
          </Route>
          <Route path="/detailed-view/year/:year/month/:month" component={DetailedRoute}>
          </Route>
          <Route path="/monthly-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth" component={MonthlyRoute} />
          <Route path="/prediction-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth/prediction-year/:predictionYear/prediction-month/:predictionMonth" component={PredictionRoute} />
          <AuthorizeRoute>
            <Route path="/allitems-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth" component={AllItemsRoute}>
            </Route>
          </AuthorizeRoute>
          <Route exact path="/">
            <Welcome />
          </Route>
        </Switch>
      </Router>
    </div>
  );
}

export default App;

import * as React from 'react';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";
import { useAuth } from "react-oidc-context";

import Welcome from "./Welcome";
import MonthlyRoute from "./MonthlyRoute";
import PredictionRoute from "./PredictionRoute";
import DetailedRoute from "./DetailedRoute";
import ItemizedRoute from "./ItemizedRoute";
import AllItemsRoute from './AllItemsRoute';

function App() {
  const auth = useAuth();

  switch (auth.activeNavigator) {
    case "signinSilent":
    return (
      <>
        <h1 className="main-header">Financier</h1>
        <div>Signing you in</div>
      </>
    );
    case "signoutRedirect":
    return (
      <>
        <div>Signing you out</div>
      </>
    );
  }

  if (auth.isLoading) {
    return (
      <>
        <h1 className="main-header">Financier</h1>

        <div>Is loading</div>;
      </>
    )
  }

  if (auth.error) {
    return (
      <>
        <h1 className="main-header">Financier</h1>

        <div>There is an error {auth.error.message}</div>
      </>
    );
  }

  if (auth.isAuthenticated) {
    return (
      <>
        <h1 className="main-header">Financier</h1>

        <Router>
          <Switch>
            <Route path="/login">
              <h2>Redirecting to login page</h2>
            </Route>
            <Route path="/itemized-view/year/:year/month/:month/tagNames/:tags" component={ItemizedRoute}>
            </Route>
            <Route path="/detailed-view/year/:year/month/:month" component={DetailedRoute}>
            </Route>
            <Route path="/monthly-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth" component={MonthlyRoute} />
            <Route path="/prediction-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth/prediction-year/:predictionYear/prediction-month/:predictionMonth" component={PredictionRoute} />
            <Route path="/allitems-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth" component={AllItemsRoute}>
            </Route>
            <Route path="/">
              <Welcome />
            </Route>
          </Switch>
        </Router>
      </>
    );
  }

  return <button onClick={() => {alert("hello"); void auth.signinRedirect()}}>Log in</button>;
}

export default App;

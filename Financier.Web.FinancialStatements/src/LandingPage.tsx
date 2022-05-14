import * as React from 'react';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";
import { useAuth } from "react-oidc-context";

import Welcome from "./Welcome";
import MonthlyRoute from "./MonthlyRoute";
import PredictionRoute from "./PredictionRoute";
import DetailedRoute from "./DetailedRoute";
import ItemizedRoute from "./ItemizedRoute";
import AllItemsRoute from './AllItemsRoute';

function LandingPage() {
  const auth = useAuth();

  var message = () => {
    if (auth.isLoading) {
      return "Is loading";
    }

    switch (auth.activeNavigator) {
      case "signinSilent":
      return "Signing you in";
      case "signoutRedirect":
      return "Signing you out";
    }
  };

  switch (auth.activeNavigator || auth.isLoading) {
    case "signinSilent":
    return (
      <>
        <h1 className="main-header">Financier</h1>
        <div className="navigation centred">
          <div className="button disabled">Log in</div>
        </div>

        <div>Signing you in</div>
      </>
    );
    case "signoutRedirect":
    return (
      <>
        <h1 className="main-header">Financier</h1>
        <div className="navigation centred">
          <div className="button disabled">Log in</div>
        </div>

        <div>Signing you out</div>
      </>
    );
  }

  if (auth.isLoading) {
    return (
      <>
        <h1 className="main-header">Financier</h1>
        <div className="navigation centred">
          <div className="button disabled">Log in</div>
        </div>

        <div>Is loading</div>
      </>
    )
  }

  if (auth.error) {
    return (
      <>
        <h1 className="main-header">Financier</h1>
        <div className="navigation centred">
          <div className="button enabled" onClick={() => {auth.signinRedirect()}}>Log in</div>
        </div>

        <div>There is an error</div>
        <div>{auth.error.message}</div>
      </>
    );
  }

  if (auth.isAuthenticated) {
    return (
      <>
        <h1 className="main-header">Financier</h1>

        <div className="navigation">
          <div>Account</div>

          <div className="button enabled" onClick={async () => {
            await auth.removeUser()

            window.location.pathname = "/";
          }}>
            Log out
          </div>
        </div>

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

  return (
    <>
      <h1 className="main-header">Financier</h1>
      <div className="navigation centred">
        <div className="button enabled" onClick={async () => {
          await auth.signinRedirect();
        }}>
          Log in
        </div>
      </div>
    </>
  );
}

export default App;

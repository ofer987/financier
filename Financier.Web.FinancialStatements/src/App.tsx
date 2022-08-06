import * as React from 'react';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";
import { useAuth } from "react-oidc-context";

import AccountNavigation from "./AccountNavigation";
import Welcome from "./Welcome";
import MonthlyRoute from "./MonthlyRoute";
import PredictionRoute from "./PredictionRoute";
import DetailedRoute from "./DetailedRoute";
import ItemizedRoute from "./ItemizedRoute";
import AllItemsRoute from './AllItemsRoute';

function App() {
  const auth = useAuth();

  if (auth.isAuthenticated) {
    return (
      <>
        <h1 className="main-header">Financier</h1>

        <div className="account-navigation">
          <AccountNavigation />
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

  const loginUrl = new URL("https://accounts.google.com/o/oauth2/v2/auth?client_id=866988055772-91t42ig7ti5sj27hk3d45b945mp8vku6.apps.googleusercontent.com&redirect_uri=https%3A%2F%2Flocalhost%3A7168%2Fredirect&response_type=code&scope=openid+profile+email&state=ab55a44afc64406eaddbd53330ffd435&code_challenge=hJ7oAGl3ZtIWBGm402y6IAKnLbcuAWjTThh2J64SvxQ&code_challenge_method=S256&response_mode=query");
  return (
    <>
      <h1 className="main-header">Financier</h1>
      <div className="account-navigation">
        <Router>
          <Switch>
            <Route>
              <div className="buttons">
                <div className="button enabled">
                  <a href={loginUrl.toString()}>
                    Log in
                  </a>
                </div>
              </div>
            </Route>
            <Route path="/login">
              <div className="buttons">
                <div className="button enabled">
                  <a href={loginUrl.toString()}>
                    Log in
                  </a>
                </div>
              </div>
            </Route>
          </Switch>
        </Router>
      </div>
    </>
  );

  // return (
  //   <>
  //     <h1 className="main-header">Financier</h1>
  //
  //     <div className="account-navigation">
  //       <AccountNavigation />
  //     </div>
  //   </>
  // );
}

export default App;

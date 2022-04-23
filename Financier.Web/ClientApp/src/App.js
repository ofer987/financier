import * as React from 'react';
// import * as ReactDOM from 'react-dom';
import { BrowserRouter as Router, Routes, Route, useParams, useLocation, Link, NavLink } from "react-router-dom";

import { Layout } from './components/Layout';
// import { Home } from './components/Home';
// import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
// import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
// import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import Welcome from "./Welcome";
// import WelcomeError from "./WelcomeError";
import MonthlyRoute from "./MonthlyRoute";
import PredictionRoute from "./PredictionRoute";
import DetailedRoute from "./DetailedRoute";
import ItemizedRoute from "./ItemizedRoute";
import AllItemsRoute from './AllItemsRoute';
// import AuthorizeRoute from './auth/components/api-authorization/AuthorizeRoute';

function App() {
  return (
    <div>
      <h1 className="main-header">Financier</h1>
      <Routes>
        <Layout>
          <Route path="/itemized-view/year/:year/month/:month/tagNames/:tags" element={ItemizedRoute}>
          </Route>
          <Route path="/detailed-view/year/:year/month/:month" element={DetailedRoute}>
          </Route>
          <Route path="/monthly-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth" element={MonthlyRoute} />
          <Route path="/prediction-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth/prediction-year/:predictionYear/prediction-month/:predictionMonth" element={PredictionRoute} />
          <Route path="/allitems-view/from-year/:fromYear/from-month/:fromMonth/to-year/:toYear/to-month/:toMonth" element={AllItemsRoute} />
          <Route path="/welcome">
            <Welcome />
            <div>hello</div>
          </Route>
        </Layout>
      </Routes>
    </div>
  );
}

export default App;

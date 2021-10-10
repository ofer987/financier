// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation } from "react-router-dom";

import App from "./App";

// CSS
import "./index.scss";

var root = document.querySelector(".root");
ReactDOM.render(
  <Router>
    <App />
  </Router>,
  root
);

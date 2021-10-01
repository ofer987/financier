'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import MonthlyCashFlow from "./MonthlyCashFlow";

// CSS
import "./index.scss";

var root = document.querySelector(".root");
ReactDOM.render(<MonthlyCashFlow year={2019} />, root);

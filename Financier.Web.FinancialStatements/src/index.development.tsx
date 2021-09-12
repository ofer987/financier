'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import CashFlow from "./CashFlow";

// CSS
import "./index.scss";

var root = document.querySelector(".root");
ReactDOM.render(<CashFlow year={2019} month={1} />, root);

'use strict';

// import 'react-app-polyfill/ie11';
import * as React from 'react';
import * as ReactDOM from 'react-dom';

// CSS
// import "./index.scss";

// JavaScript
// import ContactMe from "./contactMe";
// import Messages from "./messages";

// Configuration
// import CONFIGURATION from "./configuration.development";

import { MonthlyCashFlow } from "./MonthlyCashFlow";

(document.getElementById("root") || { textContent: ""}).textContent = "hello";

ReactDOM.render(<MonthlyCashFlow revenues={[]} expenses={[]} />, document.getElementById('root'));

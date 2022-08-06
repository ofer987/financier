import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AuthProvider } from "react-oidc-context";
import { User, WebStorageStateStore } from "oidc-client-ts";

// import * as serviceWorkerRegistration from './serviceWorkerRegistration.js';

import App from "./App";

// CSS
import "./index.scss";

const oidcConfig = {
  authority: "https://accounts.google.com/",
  client_id: "866988055772-91t42ig7ti5sj27hk3d45b945mp8vku6.apps.googleusercontent.com",
  redirect_uri: "https://localhost:7168/login",
  scope: "openid profile email",
  client_secret: "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",
  userStore: new WebStorageStateStore({ store: window.localStorage })
};

const root = document.querySelector(".root");
ReactDOM.render(
  <AuthProvider {...oidcConfig}>
    <App />
  </AuthProvider>,
  root
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
// serviceWorkerRegistration.unregister();

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AuthProvider } from "react-oidc-context";
import { User, WebStorageStateStore } from "oidc-client-ts";

// import * as serviceWorkerRegistration from './serviceWorkerRegistration.js';

import App from "./App";
import AccountNavigation from "./AccountNavigation";

// CSS
import "./index.scss";

const oidcConfig = {
  authority: "https://localhost:5001",
  client_id: "financier_client_0.0.1",
  redirect_uri: "https://localhost:7168/app",
  scope: "openid profile email",
  client_secret: "511536EF-F270-4058-80CA-1C89C192F69A",
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  onSigninCallback: (_user: User) => {
    window.location.search = "";
    window.location.hash = "";

    return;
  }
};

const root = document.querySelector(".root");
ReactDOM.render(
  <AuthProvider {...oidcConfig}>
    <h1 className="main-header">Financier</h1>

    <div className="account-navigation">
      <AccountNavigation />
    </div>

    <App />
  </AuthProvider>,
  root
);

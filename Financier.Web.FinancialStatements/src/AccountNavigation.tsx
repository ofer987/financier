import * as React from 'react';
import { BrowserRouter as Router, Switch, Route, useParams, useLocation, useRouteMatch, Link, NavLink } from "react-router-dom";
import { useAuth } from "react-oidc-context";

function Navigation() {
  const auth = useAuth();

  if (auth.isAuthenticated) {
    return (
      <>
        <div className="navigation">
          <div className="buttons">
            <div className="button disabled">Account</div>
            <div className="button enabled" onClick={async () => {
              await auth.removeUser()

              window.location.pathname = "/";
            }}>
              Log out
            </div>
          </div>

          <div className="content">
          </div>
        </div>
      </>
    );
  }

  switch (auth.activeNavigator) {
    case "signinSilent":
    return (
      <>
        <div className="navigation">
          <div className="buttons">
            <div className="button disabled">Log in</div>
          </div>

          <div className="content">
            <div>Signing you in2</div>
          </div>
        </div>
      </>
    );
    case "signoutRedirect":
    return (
      <>
        <div className="navigation">
          <div className="buttons">
            <div className="button disabled">Log in</div>
          </div>

          <div className="content">
            <div>Signing you out</div>
          </div>
        </div>
      </>
    );
  }

  if (auth.isLoading) {
    return (
      <>
        <div className="navigation">
          <div className="buttons">
            <div className="button disabled">Log in</div>
          </div>

          <div className="content">
            <div>Signing you in</div>
          </div>
        </div>
      </>
    );
  }

  if (auth.error) {
    return (
      <>
        <div className="navigation">
          <div className="buttons">
            <div className="button enabled" onClick={() => {auth.signinRedirect()}}>Log in</div>
          </div>

          <div className="content">
            <div>There is an error</div>
            <div>{auth.error.message}</div>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      <div className="navigation">
        <div className="buttons">
          <div className="button enabled" onClick={async () => {
            await auth.signinRedirect();
          }}>
            Log in
          </div>
        </div>

        <div className="content">
        </div>
      </div>
    </>
  );
}

export default Navigation;

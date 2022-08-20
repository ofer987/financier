import * as React from 'react';
import { useAuth } from "react-oidc-context";

function AccountNavigation() {
  const auth = useAuth();
  if (auth.isAuthenticated) {
    return (
      <div className="account-navigation">
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
    );
  }

  switch (auth.activeNavigator) {
    case "signinSilent":
    return (
      <>
        <div className="buttons">
          <div className="button disabled">Log in</div>
        </div>

        <div className="content">
          <div>Signing you in2</div>
        </div>
      </>
    );
    case "signoutRedirect":
    return (
      <>
        <div className="buttons">
          <div className="button disabled">Log in</div>
        </div>

        <div className="content">
          <div>Signing you out</div>
        </div>
      </>
    );
  }

  if (auth.isLoading) {
    return (
      <>
        <div className="buttons">
          <div className="button disabled">Log in</div>
        </div>

        <div className="content">
          <div>Signing you in</div>
        </div>
      </>
    );
  }

  if (auth.error) {
    return (
      <>
        <div className="buttons">
          <div className="button enabled" onClick={() => {auth.signinRedirect()}}>Log in</div>
        </div>

        <div className="content">
          <div>There is an error</div>
          <div>{auth.error.message}</div>
        </div>
      </>
    );
  }

  return (
    // Make a fetch request to OAuth2 server
    // new fetch()
    <>
      <div className="buttons">
        <div className="button enabled" onClick={async () => auth.signinSilent()}>Log in</div>
      </div>

      <div className="content">
      </div>
    </>
  );
}

export default AccountNavigation;

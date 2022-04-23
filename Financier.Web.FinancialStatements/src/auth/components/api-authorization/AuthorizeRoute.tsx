import * as React from "React";
import authService from './AuthorizeService'

// interface State {
//   ready: boolean;
//   authenticated: boolean;
// }

export default class AuthorizeRoute extends React.Component<any, any> {
  _subscription: number;
  ready: boolean;
  authenticated: boolean;

  constructor(props: any) {
    super(props);

    this.state = {
      ready: false,
      authenticated: false
    };
  }

  componentDidMount() {
    this._subscription = authService.subscribe(() => this.authenticationChanged());
    this.populateAuthenticationState();
  }

  componentWillUnmount() {
    authService.unsubscribe(this._subscription);
  }

  render() {
    const { ready, authenticated } = this.state;
    var link = document.createElement("a");
    link.href = this.props.path;
    // const returnUrl = `${link.protocol}//${link.host}${link.pathname}${link.search}${link.hash}`;
    // const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURIComponent(returnUrl)}`
    if (!ready || !authenticated) {
      return <div></div>;
    } else {
      return (
        <>
          {this.props.children}
        </>
      )
      //
      // return this.props.children;
    }
  }

  async populateAuthenticationState() {
    const authenticated = await authService.isAuthenticated();
    this.setState({ ready: true, authenticated });
  }

  async authenticationChanged() {
    this.setState({ ready: false, authenticated: false });
    await this.populateAuthenticationState();
  }
}

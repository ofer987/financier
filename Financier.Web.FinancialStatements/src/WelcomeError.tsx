import React from "react";
import "WelcomeError.scss";

interface Props {}

interface State {
  hasError: boolean;
  message: string | undefined;
}

class WelcomeError extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      hasError: false,
      message: undefined,
    };
  }

  componentDidCatch(error: Error, _errorInfo: React.ErrorInfo) {
    this.setState({
      hasError: true,
      message: error.message,
    });
  }

  render() {
    if (this.state.hasError) {
      this.setState({
        hasError: false
      });

      return (
        <div className="WelcomeError">
          Oh no, an error! {this.state.message}
          {this.props.children}
        </div>
      )
    }

    return this.props.children;
  }
}

export default WelcomeError;

import React, { useState } from "react";
import * as d3TimeFormat from "d3-time-format";

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

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
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
        <div>
          Oh no, an error! {this.state.message}
          {this.props.children}
        </div>
      )
    }

    return this.props.children;
  }
}

export default WelcomeError;

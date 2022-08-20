import * as React from "react";
import { useAuth } from "react-oidc-context";

import { MonthlyCashFlow, Props as MonthlyProps } from "./MonthlyCashFlow";

interface Props {
  match: {
    params: MonthlyProps
  }
}

const MonthlyRoute = (props: Props) => {
  const auth = useAuth();
  const token = auth 
    ? auth?.user?.id_token 
    : null;

  if (token) {
    return (
      <MonthlyCashFlow token={token} {...props.match.params} />
    );
  }

  return <></>;
}

export default MonthlyRoute;

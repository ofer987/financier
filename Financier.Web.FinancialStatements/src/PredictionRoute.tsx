import * as React from "react";
import { useAuth } from "react-oidc-context";

import { PredictionCashFlow, Props as PredictionProps } from "./PredictionCashFlow";

interface Props {
  match: {
    params: PredictionProps 
  }
}

const PredictionRoute = (props: Props) => {
  const auth = useAuth();
  const token = auth 
    ? auth?.user?.id_token 
    : null;

  if (token) {
    return (
      <PredictionCashFlow token={token} {...props.match.params} />
    );
  }

  return <></>;
}

export default PredictionRoute;

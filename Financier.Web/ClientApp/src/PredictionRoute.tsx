import * as React from "react";

import { PredictionCashFlow, PredictionProps } from "./PredictionCashFlow";

interface Props {
  match: {
    params: PredictionProps 
  }
}

const PredictionRoute = (props: Props) => {
  return (
    <PredictionCashFlow {...props.match.params} />
  );
}

export default PredictionRoute;
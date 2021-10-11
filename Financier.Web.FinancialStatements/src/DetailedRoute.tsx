import * as React from "react";

import DetailedCashFlow from "./DetailedCashFlow";

interface Props {
  match: {
    params: {
      year: number;
      month: number;
    }
  }
}

const DetailedRoute = ( props: Props ) => {
  return (
    <DetailedCashFlow year={props.match.params.year} month={props.match.params.month} />
  );
}

export default DetailedRoute;

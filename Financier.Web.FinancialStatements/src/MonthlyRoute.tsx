import * as React from "react";

import MonthlyCashFlow from "./MonthlyCashFlow";

interface Props {
  match: {
    params: {
      year: number;
    }
  }
}

const MonthlyRoute = ( props: Props ) => {
  console.log(`Monthly Route: ${props.match.params.year}`);

  return (
    <MonthlyCashFlow year={props.match.params.year} />
  );
}

export default MonthlyRoute;

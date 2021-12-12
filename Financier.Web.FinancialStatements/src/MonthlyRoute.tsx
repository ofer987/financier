import * as React from "react";

import { MonthlyCashFlow, Props as MonthlyProps } from "./MonthlyCashFlow";

interface Props {
  match: {
    params: MonthlyProps
  }
}

const MonthlyRoute = (props: Props) => {
  alert(`${props.match.params.fromMonth}: `);
  return (
    <MonthlyCashFlow {...props.match.params} />
  );
}

export default MonthlyRoute;

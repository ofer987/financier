import * as React from "react";

import { AllItemsCashFlow, Props as AllItemsProps } from "./AllItemsCashFlow";

interface Props {
  match: {
    params: AllItemsProps;
  }
}

const AllItemsRoute = (props: Props) => {
  return (
    <AllItemsCashFlow {...props.match.params} />
  );
}

export default AllItemsRoute;

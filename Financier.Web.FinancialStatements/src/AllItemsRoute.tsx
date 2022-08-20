import * as React from "react";
import { useAuth } from "react-oidc-context";

import { AllItemsCashFlow, Props as AllItemsProps } from "./AllItemsCashFlow";

interface Props {
  match: {
    params: AllItemsProps;
  }
}

const AllItemsRoute = (props: Props) => {
  let auth = useAuth();

  const token = auth 
    ? auth?.user?.id_token 
    : null;

  if (token) {
    return (
      <AllItemsCashFlow token={token} {...props.match.params} />
    );
  }

  return <></>;
}

export default AllItemsRoute;

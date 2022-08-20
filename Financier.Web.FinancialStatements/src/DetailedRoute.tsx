import * as React from "react";
import { useAuth } from "react-oidc-context";

import { DetailedCashFlow } from "./DetailedCashFlow";

interface Props {
  match: {
    params: {
      year: number;
      month: number;
    }
  }
}

const DetailedRoute = (props: Props) => {
  const auth = useAuth();
  const token = auth 
    ? auth?.user?.id_token 
    : null;

  if (token) {
    return (
      <DetailedCashFlow token={token} year={props.match.params.year} month={props.match.params.month} />
      );
  }

  return <></>;
}

export default DetailedRoute;

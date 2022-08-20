import * as React from "react";
import { kebabCase } from "lodash";
import { useAuth } from "react-oidc-context";

import { ItemizedCashFlow } from "./ItemizedCashFlow";

interface Props {
  match: {
    params: {
      year: number;
      month: number;
      tags: string;
    }
  }
}

const ItemizedRoute = (props: Props) => {
  const auth = useAuth();
  const token = auth 
    ? auth?.user?.id_token 
    : null;

  if (token) {
    const year = props.match.params.year;
    const month = props.match.params.month;

    console.log(props.match.params.tags);
    const tags = props.match.params.tags
      .split(",")
      .map(item => item.trim())
      .map(item => kebabCase(item));
    console.log(tags);

    return (
      <ItemizedCashFlow token={token} year={year} month={month} tagNames={tags} />
    );
  }

  return <></>;
}

export default ItemizedRoute;

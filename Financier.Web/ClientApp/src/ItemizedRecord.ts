import { Amount } from "./Amount";

interface ItemizedRecord {
  id: string,
  name: string;
  at: string;
  amount: Amount;
  tags: string[];
}

export { ItemizedRecord };

import { Amount } from "./Amount";

interface ItemizedRecord {
  name: string;
  at: string;
  amount: Amount;
  tags: string[];
}

export { ItemizedRecord };

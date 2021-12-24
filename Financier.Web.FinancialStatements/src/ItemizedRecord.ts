import { Amount } from "./Amount";

interface ItemizedRecord {
  name: string;
  at: Date;
  amount: Amount;
  tags: string[];
}

export { ItemizedRecord };

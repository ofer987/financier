import { Amount } from "./Amount";

interface MonthlyRecord {
  isPrediction: boolean;
  year: number;
  month: number;
  amount: Amount;
}

export { MonthlyRecord };

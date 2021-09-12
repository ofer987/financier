import Listing from "./CashFlowModel/Listing";

class CashFlowModel {
  startAt: string;
  endAt: string;
  debitListings: Listing[];
  creditListings: Listing[];
}

export default CashFlowModel;

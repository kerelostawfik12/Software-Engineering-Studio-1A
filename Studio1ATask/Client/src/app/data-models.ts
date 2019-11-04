// See DataModels.cs for the server-side version of these classes (make sure they are consistent)
interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface TestModel
{
  id : number;
  aNumber : number;
  aString : string;
}

interface ForeignKeyTest
{
  id : number;
  testModelId : number;
  testModel : TestModel;
}

interface Seller {
  id : number;
  name : string;
  account : Account;
}

interface Account{
  id: string;
  email: string;
  firstName: string;
  lastName: string

}

interface Item {
  id : number;
  name : string;
  description : string;
  sellerId : number;
  seller : Seller;
  price : number;
  imageURL: string;
  views? : number;
}

interface Customer {
  id : number;
  firstName : string;
  lastName : string;
  email: string;
  account: Account;

}

interface CustomerTransaction {
  customerName: string;
  id: number;
  customerId?: Customer;
  date: Date;
  total: number;
  paypalTransactionId: string;
}

interface TransactionItem {
  id: number;
  customerTransaction: CustomerTransaction;
  itemSaleId: number;
  itemSalePrice: number;
  itemSaleName: string;
  sellerSaleId: number;
  sellerSaleName: string;
}


interface SearchItemResult {
  items: Item[]; // The mostViewed returned by the query
  categories?: JSON; // All categories of mostViewed returned by the query
  sellers?: JSON; // All sellers of mostViewed returned by the query
  brands?: JSON; // All brands of mostViewed returned by the query
  // Categories/sellers/brands JSON will probably look like this:
  /*
  {
    4: {
      id = 4,
      name = "Desktop PCs",
      numResults = 2
    }
  }
  */
  maxPrice: number; // The max price of mostViewed returned by the query
  minPrice: number; // The minimum price of mostViewed returned by the query
  numItems: number; // The total number of mostViewed fitting the query
}

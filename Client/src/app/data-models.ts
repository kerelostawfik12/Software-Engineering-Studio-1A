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
}

interface Item {
  id : number;
  name : string;
  description : string;
  sellerId : number;
  seller : Seller;
  price : number;
}


interface SearchItemResult {
  items: Item[]; // The items returned by the query
  categories?: JSON; // All categories of items returned by the query
  sellers?: JSON; // All sellers of items returned by the query
  brands?: JSON; // All brands of items returned by the query
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
  maxPrice: number; // The max price of items returned by the query
  minPrice: number; // The minimum price of items returned by the query
  numItems: number; // The total number of items fitting the query
}
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

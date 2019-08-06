import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];
  public testModels: TestModel[];
  public foreignKeyTests: ForeignKeyTest[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<WeatherForecast[]>(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));

    http.get<TestModel[]>(baseUrl + 'api/TestData/TestModels').subscribe(result => {
      this.testModels = result;
    }, error => console.error(error));

    http.get<ForeignKeyTest[]>(baseUrl + 'api/TestData/ForeignKeyTests').subscribe(result => {
      this.foreignKeyTests = result;
    }, error => console.error(error));

  }
}

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




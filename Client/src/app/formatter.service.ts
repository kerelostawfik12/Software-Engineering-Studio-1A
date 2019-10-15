import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FormatterService {

  constructor() { }

  public convertToMoneyString(input: number): string {
    return this.addTrailingZero(input.toLocaleString())
  }

  addZero(input: any): string {
    let str = input.toString();
    if (str.length == 1)
      return "0" + str;
    if (str.length == 0)
      return "00";
    return str;
  }

  addTrailingZero(input: string): string {
    let split = input.split('.');
    if (split.length > 1 && split[split.length - 1].length == 1)
      return input + "0";
    else if (split.length == 1)
      return input + ".00";
    return input;
  }


}

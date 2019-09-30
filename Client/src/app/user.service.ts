import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

export class User {
  public isLoaded: boolean = false;
  public isLoggedIn: boolean = false;
  public data : any = null;

  public get firstName(): string {
    if (this.data.hasOwnProperty("firstName")) {
      return this.data["firstName"];
    } else
      return this.data["name"];
  }
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private static current: User = new User();

  public getCurrent(): Observable<User> {
    return of(UserService.current);
  }
  public setCurrent(data : Object, isLoaded : boolean, isLoggedIn : boolean) {
    UserService.current.data = data;
    UserService.current.isLoggedIn = isLoggedIn;
    UserService.current.isLoaded = isLoaded;
  }

  public ifSeller(): boolean{
    return UserService.current.data["type"] == 's';
  }

  public ifCustomer(): boolean{
    return UserService.current.data["type"] == 'c';
  }







  constructor() { }
}



import {Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';

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

  public get isSeller(): boolean {
    return this.data != null && this.data["type"] == 's';
  }

  public get isCustomer(): boolean {
    return this.data != null && this.data["type"] == 'c';
  }

  public get isAdmin(): boolean {
    return this.data != null && this.data["type"] == 'a';
  }
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private static current: User = new User();
  private static currentObservable = of(UserService.current);

  public getCurrent(): Observable<User> {
    return UserService.currentObservable;
  }
  public setCurrent(data : Object, isLoaded : boolean, isLoggedIn : boolean) {
    UserService.current.data = data;
    UserService.current.isLoggedIn = isLoggedIn;
    UserService.current.isLoaded = isLoaded;
  }


  constructor() { }
}



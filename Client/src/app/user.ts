export class User {
  public static current: User = null;
  public isLoaded: boolean = false;
  public isLoggedIn: boolean = false;
  public data = null;

  public get firstName(): string {
    if (this.data.hasOwnProperty("firstName")) {
      return this.data["firstName"];
    } else
      return this.data["name"];
  }
}

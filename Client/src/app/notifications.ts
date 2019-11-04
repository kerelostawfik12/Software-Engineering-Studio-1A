import AWN from "awesome-notifications"
// Set global options
let globalOptions = {};
// Initialize instance of AWN
let notifier = new AWN(globalOptions);

// Wrapper class for notifications and stuff
export class Notifications {
  public notifier: any = notifier;

  public static success(text: any, optionsOverrides: Object = {}) {
    notifier.success(text.toString(), optionsOverrides);
  }

  public static warning(text: any, optionsOverrides: Object = {}) {
    notifier.warning(text.toString(), optionsOverrides);
  }

  public static info(text: any, optionsOverrides: Object = {}) {
    notifier.info(text.toString(), optionsOverrides);
  }

  public static error(text: any, optionsOverrides: Object = {}) {
    notifier.alert(text.toString(), optionsOverrides);
  }

  public static confirm(text: string, onOk: Function, onCancel: Function) {
    notifier.confirm(
      text,
      onOk,
      onCancel,
      {
        labels: {
          confirm: 'Dangerous action'
        }
      }
    )
  }

}

import AWN from "awesome-notifications"
// Set global options
let globalOptions = {};
// Initialize instance of AWN
let notifier = new AWN(globalOptions);

// Wrapper class for notifications and stuff
export class Notifications {
  public notifier: any = notifier;

  public static success(text: string, optionsOverrides: Object = {}) {
    notifier.success(text, optionsOverrides);
  }

  public static warning(text: string, optionsOverrides: Object = {}) {
    notifier.warning(text, optionsOverrides);
  }

  public static info(text: string, optionsOverrides: Object = {}) {
    notifier.info(text, optionsOverrides);
  }

  public static error(text: string, optionsOverrides: Object = {}) {
    notifier.alert(text, optionsOverrides);
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

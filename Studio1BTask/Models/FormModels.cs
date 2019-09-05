namespace Studio1BTask.Models
{
    public class CustomerAccountForm
    {
        public string Email;
        public string FirstName;
        public string LastName;
        public string Password;
    }

    public class NewItemForm
    {
        public string Description;
        public string Name;
        public string Price;
    }

    // You can't just pass a simple variable in ASP, so wrapping primitives in an object is necessary.
    // Yes, it's dumb.
    public class SimpleInt
    {
        public int Value;
    }
}
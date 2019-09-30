using System.Collections.Generic;

namespace Studio1BTask.Models
{
    public class CustomerAccountForm
    {
        public string Email;
        public string FirstName;
        public string LastName;
        public string Password;
    }

    public class SellerAccountForm
    {
        public string Name;
        public string Password;
        public string Email;
    }

    public class LoginForm
    {
        public string Email;
        public string Password;
    }

    public class NewItemForm
    {
        public string Description;
        public string Name;
        public string Price;
    }


    public class SearchItemResult
    {
        public dynamic Brands;
        public dynamic Categories;
        public IEnumerable<Item> Items;
        public decimal MaxPrice;
        public decimal MinPrice;
        public dynamic Sellers;
    }

    // You can't just pass a simple variable in ASP, so wrapping primitives in an object is necessary.
    // Yes, it's dumb.
    public class SimpleInt
    {
        public int Value;
    }

    public class SimpleFloat
    {
        public float Value;
    }

    public class SimpleDecimal
    {
        public decimal Value;
    }

    public class SimpleString
    {
        public string Value;
    }

    public class SimpleChar
    {
        public string Value;
    }

    public class SimpleBool
    {
        public bool Value;
    }
}
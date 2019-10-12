using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;
using Studio1BTask.Models;
using Studio1BTask.Services;
using DbContext = Studio1BTask.Models.DbContext;
using Item = PayPalCheckoutSdk.Orders.Item;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly AccountService _accountService = new AccountService();
        private readonly PaypalService _paypalService = new PaypalService();

        // If the requesting user is a customer, it will get all transactions from that customer. 
        // If the requesting user is a seller, it will get all transactions from that seller. 
        // If the requesting user is an admin, it will get all transactions in the database.
        [HttpGet("[action]")]
        public IEnumerable<TransactionItem> AllTransactions()
        {
            using (var context = new DbContext())
            {
                // If user is a customer, get all their transactions.
                var customer = _accountService.ValidateCustomerSession(Request.Cookies, context);
                if (customer != null)
                {
                    var items = context.TransactionItems
                        .Include(x => x.CustomerTransaction)
                        .Where(x => x.CustomerTransaction.CustomerId == customer.Id)
                        .OrderByDescending(x => x.CustomerTransaction.Date)
                        .ToList();
                    return items;
                }

                // If user is a seller, get all their transactions.
                var seller = _accountService.ValidateSellerSession(Request.Cookies, context);
                if (seller != null)
                {
                    var items = context.TransactionItems
                        .Where(x => x.SellerSaleId == seller.Id)
                        .Include(x => x.CustomerTransaction)
                        .OrderByDescending(x => x.CustomerTransaction.Date)
                        .ToList();
                    return items;
                }

                // If user is an admin, get everything.
                if (_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    var items = context.TransactionItems
                        .Include(x => x.CustomerTransaction)
                        .OrderByDescending(x => x.CustomerTransaction.Date)
                        .ToList();
                    return items;
                }

                // If the user is not a seller, customer, or admin, there is nothing to give them.
                Response.StatusCode = 403;
                return null;
            }
        }

        [HttpPost("[action]")]
        public async Task<Order> CreatePaypalOrder()
        {
            var orderItems = new List<Item>();
            decimal totalPrice = 0;
            using (var context = new DbContext())
            {
                var customer = _accountService.ValidateCustomerSession(Request.Cookies, context, true);
                var itemsInCart = context.CartItems
                    .Where(x => x.SessionId == customer.Account.SessionId)
                    .Include(x => x.Item)
                    .Select(cartItem => cartItem.Item).Include(x => x.Seller)
                    .ToList();

                foreach (var item in itemsInCart)
                {
                    totalPrice += item.Price;
                    orderItems.Add(new Item
                    {
                        Name = item.Name,
                        Sku = item.Id.ToString(),
                        Quantity = 1.ToString(),
                        Category = "PHYSICAL_GOODS",
                        Description = "Sold by " + item.Seller.Name + ". " + item.Description,
                        Tax = new Money
                        {
                            CurrencyCode = "AUD",
                            Value = "0.00"
                        },
                        UnitAmount = new Money
                        {
                            CurrencyCode = "AUD",
                            Value = item.Price.ToString(CultureInfo.InvariantCulture)
                        }
                    });
                }
            }

            // Create items for order 
            var order = await _paypalService.CreateOrder(orderItems, totalPrice);
            return order;
        }

        [HttpGet("[action]")]
        public async Task<Order> CapturePaypalOrder([FromQuery] string orderId)
        {
            using (var context = new DbContext())
            {
                // Refuse capture if customer is not properly authenticated, or if server does not remember creating the order
                var customer = _accountService.ValidateCustomerSession(Request.Cookies, context, true);
                if (customer == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    return null;
                }

                // Capture funds through Paypal
                var order = await _paypalService.CaptureOrder(orderId);

                // Create transaction object
                var transaction = context.CustomerTransactions.Add(new CustomerTransaction
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.FirstName + " " + customer.LastName,
                    Date = DateTime.UtcNow.AddHours(10), // Force Sydney timezone
                    PaypalTransactionId = order.Id,
                    Total = decimal.Parse(order.PurchaseUnits[0].AmountWithBreakdown.Value)
                });
                // Create transaction item objects
                var items = order.PurchaseUnits[0].Items;
                foreach (var item in items)
                {
                    var itemId = int.Parse(item.Sku);
                    var itemEntity = context.Items
                        .Include(x => x.Seller)
                        .First(x => x.Id == itemId);
                    context.TransactionItems.Add(new TransactionItem
                    {
                        CustomerTransaction = transaction.Entity,
                        ItemSaleId = itemId,
                        ItemSalePrice = decimal.Parse(item.UnitAmount.Value),
                        ItemSaleName = item.Name,
                        SellerSaleId = itemEntity.SellerId,
                        SellerSaleName = itemEntity.Seller.Name
                    });
                }

                // Clear user's shopping cart
                var itemsToRemove = context.CartItems.Where(x => x.SessionId == customer.Account.SessionId);
                context.CartItems.RemoveRange(itemsToRemove);
                context.SaveChanges();
                return order;
            }
        }
    }
}
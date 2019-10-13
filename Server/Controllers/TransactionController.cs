using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
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
                    .Where(x => !x.Item.Hidden)
                    .Select(cartItem => cartItem.Item).Include(x => x.Seller)
                    .ToList();

                foreach (var item in itemsInCart)
                {
                    totalPrice += item.Price;

                    // Limit description and title size so that Paypal doesn't refuse it
                    const int maxStrSize = 100;
                    var itemDescription = "Sold by " + item.Seller.Name + ". " + item.Description;
                    if (itemDescription.Length > maxStrSize) itemDescription = itemDescription.Substring(0, maxStrSize);
                    var itemName = item.Name;
                    if (itemName.Length > maxStrSize) itemName = itemName.Substring(0, maxStrSize);

                    orderItems.Add(new Item
                    {
                        Name = itemName,
                        Sku = item.Id.ToString(),
                        Quantity = 1.ToString(),
                        Category = "PHYSICAL_GOODS",
                        Description = itemDescription,
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
        public async Task<Dictionary<string, dynamic>> CapturePaypalOrder([FromQuery] string orderId)
        {
            using (var context = new DbContext())
            {
                // Refuse capture if customer is not properly authenticated
                var customer = _accountService.ValidateCustomerSession(Request.Cookies, context, true);
                if (customer == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    return null;
                }

                // Refuse capture if 

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
                var lootBoxItems = new List<int>();
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

                    // If item is a loot box, also add in a random item below its price.
                    if (itemEntity.Id == 86)
                    {
                        var eligibleItems = context.Items
                            .Include(x => x.Seller)
                            .Where(x => !x.Hidden && x.Price < itemEntity.Price);
                        var skip = (int) (new Random().NextDouble() * eligibleItems.Count());
                        var chosenItem = eligibleItems.Skip(skip).Take(1).FirstOrDefault();
                        TransactionItem transactionItem;
                        if (chosenItem == null)
                            transactionItem = context.TransactionItems.Add(new TransactionItem
                            {
                                CustomerTransaction = transaction.Entity,
                                ItemSaleId = -1,
                                ItemSalePrice = 0,
                                ItemSaleName = "Empty Cardboard Box",
                                SellerSaleId = itemEntity.SellerId,
                                SellerSaleName = itemEntity.Seller.Name
                            }).Entity;
                        else
                            transactionItem = context.TransactionItems.Add(new TransactionItem
                            {
                                CustomerTransaction = transaction.Entity,
                                ItemSaleId = chosenItem.Id,
                                ItemSalePrice = 0,
                                ItemSaleName = chosenItem.Name + " (from Loot Box)",
                                SellerSaleId = chosenItem.SellerId,
                                SellerSaleName = chosenItem.Seller.Name
                            }).Entity;
                        Console.WriteLine("Loot box item unlocked: " + transactionItem.ItemSaleName);
                        lootBoxItems.Add(transactionItem.ItemSaleId);
                    }
                }

                // Clear user's shopping cart
                var itemsToRemove = context.CartItems.Where(x => x.SessionId == customer.Account.SessionId);
                context.CartItems.RemoveRange(itemsToRemove);
                context.SaveChanges();

                if (lootBoxItems.IsNullOrEmpty())
                    lootBoxItems = null;
                return new Dictionary<string, dynamic>
                {
                    ["order"] = order,
                    ["lootBoxItems"] = lootBoxItems
                };
            }
        }
    }
}
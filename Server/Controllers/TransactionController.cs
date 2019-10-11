using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studio1BTask.Models;
using Studio1BTask.Services;
using DbContext = Studio1BTask.Models.DbContext;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly AccountService _accountService = new AccountService();

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
                        .OrderBy(x => x.CustomerTransaction.Date)
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
                        .OrderBy(x => x.CustomerTransaction.Date)
                        .ToList();
                    return items;
                }

                // If user is an admin, get everything.
                if (_accountService.ValidateAdminSession(Request.Cookies, context))
                {
                    var items = context.TransactionItems
                        .Include(x => x.CustomerTransaction)
                        .OrderBy(x => x.CustomerTransaction.Date)
                        .ToList();
                    return items;
                }

                // If the user is not a seller, customer, or admin, there is nothing to give them.
                Response.StatusCode = 403;
                return null;
            }
        }

        [HttpPost("[action]")]
        public Dictionary<string, dynamic> CreateNewTransaction()
        {
            // Creating a new transaction involves recording all the items from the shopping cart as TransactionItems,
            // and creating the owning transaction object. Only customers can initiate transactions.
            var obj = new Dictionary<string, dynamic> {["error"] = "An unknown error occurred."};
            using (var context = new DbContext())
            {
                var customer = _accountService.ValidateCustomerSession(Request.Cookies, context, true);
                if (customer == null)
                {
                    Response.StatusCode = 401; // Unauthorised
                    obj["error"] = "Only logged in customers can initiate a transaction.";
                    return obj;
                }

                var transaction = context.CustomerTransactions.Add(new CustomerTransaction
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.FirstName + customer.LastName,
                    Date = DateTime.Now
                });

                var items = context.CartItems
                    .Where(x => x.SessionId == customer.Account.SessionId)
                    .Include(x => x.Item)
                    .Select(cartItem => cartItem.Item).Include(x => x.Seller)
                    .ToList();

                if (items.IsNullOrEmpty())
                {
                    Response.StatusCode = 412; // Precondition failed
                    obj["error"] = "You don't have any items in your cart.";
                    return obj;
                }

                foreach (var item in items)
                    context.TransactionItems.Add(new TransactionItem
                    {
                        CustomerTransaction = transaction.Entity,
                        ItemSaleId = item.Id,
                        ItemSalePrice = item.Price,
                        ItemSaleName = item.Name,
                        SellerSaleId = item.SellerId,
                        SellerSaleName = item.Seller.Name
                    });

                context.SaveChanges();

                var transactionItems
                    = context.TransactionItems.Where(x => x.TransactionId == transaction.Entity.Id).ToList();

                obj["items"] = transactionItems;
                obj["transaction"] = transaction.Entity;
                return obj;
            }
        }
    }
}
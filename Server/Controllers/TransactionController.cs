using System.Collections.Generic;
using System.Linq;
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
    }
}
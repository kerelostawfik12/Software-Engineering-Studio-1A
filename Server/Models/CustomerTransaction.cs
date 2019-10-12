using System;
using System.ComponentModel.DataAnnotations;

namespace Studio1BTask.Models
{
    public class CustomerTransaction
    {
        [Key] public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string PaypalTransactionId { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
    }
}
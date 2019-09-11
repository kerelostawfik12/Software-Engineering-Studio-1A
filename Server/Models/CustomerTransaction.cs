using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class CustomerTransaction
    {
        [Key] public int Id { get; set; }
        public virtual Customer Customer { get; set; }
        [ForeignKey("Customer")] public int CustomerId { get; set; }
        public DateTime Date { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class TransactionItem
    {
        [ForeignKey("CustomerTransaction")] public int TransactionId { get; set; }


        // This field only exists since EF core needs a unique primary key to delete things without bugging out
        [Key] public int Id { get; set; }
        public virtual CustomerTransaction CustomerTransaction { get; set; }
        public int ItemSaleId { get; set; }
        public decimal ItemSalePrice { get; set; }
        public string ItemSaleName { get; set; }
        public int SellerSaleId { get; set; }
        public string SellerSaleName { get; set; }
    }
}
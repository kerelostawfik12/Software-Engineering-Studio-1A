using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class TransactionItem
    {
        [ForeignKey("CustomerTransaction")] public int TransactionId { get; set; }

        [ForeignKey("Item")] public int ItemId { get; set; }

        // This field only exists since EF core needs a unique primary key to delete things without bugging out
        [Key] public int Id { get; set; }

        public virtual CustomerTransaction CustomerTransaction { get; set; }
        public virtual Item Item { get; set; }
        public decimal ItemSalePrice { get; set; }
    }
}
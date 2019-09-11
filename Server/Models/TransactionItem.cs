using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class TransactionItem
    {
        [ForeignKey("CustomerTransaction")]
        [Key]
        public int TransactionId { get; set; }

        [ForeignKey("Item")] [Key] public int ItemId { get; set; }
        public virtual CustomerTransaction CustomerTransaction { get; set; }
        public virtual Item Item { get; set; }
        public decimal ItemSalePrice { get; set; }
    }
}
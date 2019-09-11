using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class CustomerCartItem
    {
        public virtual Customer Customer { get; set; }
        public virtual Item Item { get; set; }
        [ForeignKey("Customer")] [Key] public int CustomerId { get; set; }
        [ForeignKey("Item")] [Key] public int ItemId { get; set; }
    }
}
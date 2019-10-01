using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class CartItem
    {
        public virtual Session Session { get; set; }
        public virtual Item Item { get; set; }

        // This field only exists since EF core needs a unique primary key to delete things without bugging out
        [Key] public int Id { get; set; }
        [ForeignKey("Session")] public int SessionId { get; set; }
        [ForeignKey("Item")] public int ItemId { get; set; }
    }
}
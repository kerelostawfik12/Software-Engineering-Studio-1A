using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class ItemCategory
    {
        [ForeignKey("Item")] [Key] public int Id { get; set; }
        public virtual Item Item { get; set; }
        public string Category { get; set; }
    }
}
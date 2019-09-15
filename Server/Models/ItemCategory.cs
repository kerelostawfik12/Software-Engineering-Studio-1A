using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class ItemCategory
    {
        [ForeignKey("Item")] [Key] public int ItemId { get; set; }
        [ForeignKey("Category")] [Key] public int CategoryId { get; set; }
        public virtual Item Item { get; set; }
        public virtual Category Category { get; set; }
    }

    public class Category
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
    }
}
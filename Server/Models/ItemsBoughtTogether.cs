using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class ItemsBoughtTogether
    {
        [ForeignKey("ItemA")] public int ItemAId { get; set; }
        [ForeignKey("ItemB")] public int ItemBId { get; set; }
        public virtual Item ItemA { get; set; }
        public virtual Item ItemB { get; set; }
        public int Count { get; set; }
    }
}
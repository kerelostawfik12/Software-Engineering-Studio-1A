using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class Item
    {
        [Key] public int Id { get; set; }


        public virtual Seller Seller { get; set; }
        [ForeignKey("Seller")] public int SellerId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string ImageURL { get; set; }
        public decimal Price { get; set; }
        public int? Views { get; set; }
        public int Purchases { get; set; }
        public bool Hidden { get; set; }
    }
}
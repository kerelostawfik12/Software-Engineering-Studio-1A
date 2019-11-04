using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class Seller
    {
        // Item Ids and Seller Ids are the same thing.
        public virtual Account Account { get; set; }
        [ForeignKey("Account")] [Key] public int Id { get; set; }

        public string Name { get; set; }
    }
}
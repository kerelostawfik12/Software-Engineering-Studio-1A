using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class Customer
    {
        // Item Ids and Customer Ids are the same thing.
        public virtual Account Account { get; set; }
        [ForeignKey("Account")] [Key] public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
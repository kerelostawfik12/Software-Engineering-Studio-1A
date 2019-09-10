using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class Customer
    {
        [Key] public int Id { get; set; }

        public virtual Account Account { get; set; }
        [ForeignKey("Account")] public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
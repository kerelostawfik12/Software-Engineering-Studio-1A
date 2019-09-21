using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class Session
    {
        [Key] public int Id { get; set; }
        public string Token { get; set; }
        public virtual Account Account { get; set; }
        [ForeignKey("Account")] public int? AccountId { get; set; }
    }
}
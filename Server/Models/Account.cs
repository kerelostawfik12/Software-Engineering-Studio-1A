using System.ComponentModel.DataAnnotations;

namespace Studio1BTask.Models
{
    public class Account
    {
        [Key] public int Id { get; set; }
        public char Type { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string AccessToken { get; set; }
    }
}
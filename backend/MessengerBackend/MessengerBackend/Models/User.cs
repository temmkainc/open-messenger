using System.ComponentModel.DataAnnotations;

namespace MessengerBackend.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}

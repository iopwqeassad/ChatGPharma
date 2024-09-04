using System.ComponentModel.DataAnnotations;

namespace pharmace.Models
{
    public class userpermations
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? fname { get; set; }
        public string? lname { get; set; }

    }
}

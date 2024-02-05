using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public virtual Profile Profile { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}

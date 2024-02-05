using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}

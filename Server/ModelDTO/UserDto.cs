using System.ComponentModel.DataAnnotations;

namespace Server.ModelDTO
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ProfileDto Profile { get; set; }
    }
}

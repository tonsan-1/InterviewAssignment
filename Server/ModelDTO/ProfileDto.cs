using System.ComponentModel.DataAnnotations.Schema;

namespace Server.ModelDTO
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int UserId { get; set; }
    }
}

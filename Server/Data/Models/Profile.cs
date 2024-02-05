using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public virtual User User { get; set; }
    }
}

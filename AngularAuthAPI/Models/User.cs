using System.ComponentModel.DataAnnotations;

namespace AngularAuthAPI.Models
{
    public class User //1. Create Custom Table
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }

    }
}

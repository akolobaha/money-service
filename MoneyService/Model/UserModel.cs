using System.ComponentModel.DataAnnotations;

namespace MoneyService.Model
{
    
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool ModerationCompleted { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace MoneyService.Model
{
    
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string UserStatus { get; set; }
        public bool ModerationCompleted { get; set; }

    }
}

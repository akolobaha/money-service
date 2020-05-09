using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyService.BuisnessLogic.Auth
{
    public class UserDb : User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool ModerationCompleted { get; set; }
        public string Salt { get; set; }
    }
}

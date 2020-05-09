using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyService.BuisnessLogic.Auth
{
    public interface IUserService
    {
        bool IsValidUser(string username, string password);
    }

    public class UserService 
    {
        public bool IsValidUser(string password, string salt, string hash)
        {
            //var testUser = new User("username", "password");



            return Password.CheckPassword(password, salt, hash);
        }
    }
}

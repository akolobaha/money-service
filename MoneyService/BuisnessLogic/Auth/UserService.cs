using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            return Password.CheckPassword(password, salt, hash);
        }

        public bool IsValidEmail(string email)
        {
            if (email != null)
                try
                {
                    return Regex.IsMatch(email,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            return false;
        }
    }
}

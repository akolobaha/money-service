using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyService.BuisnessLogic.Auth
{
    
    public class User
    {
        

        
        public User(string username, string password)
        {
            
             Username = username;
            _password = new Password(password);
        }
        
        public Password _password;
        [Key]
        public string Username { get; set; }
        public string Password => _password.PasswordHash;
        public string Salt => _password.Salt;

       
    }
}

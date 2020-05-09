using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MoneyService.BuisnessLogic.Auth;
using MoneyService.DB;
using MoneyService.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace MoneyService.Controller
{
    [Route("[controller]")]
    [ApiController]
    public  class RegisterController : ControllerBase
    {

        private IConfiguration _config;
        public RegisterController(IConfiguration config)
        {
            _config = config;
        }
        /*[HttpGet]
        public string Index(string email, string password)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                UserModel user1 = new UserModel { EmailAddress = email, Password= password, ModerationCompleted = false };

                // Добавляем объекты в БД
                if (user1.EmailAddress != null && user1.Password != null)
                {
                    db.Users.Add(user1);
                    db.SaveChanges();
                }
            }
            return "Email: " + email + " Password " + password;
        }*/

        [HttpGet]
        public string Index(string username, string password)
        {
            User user = new User(username, password);


            string sql = "INSERT INTO public.\"Users\" (\"Username\", \"Password\", \"Salt\") VALUES('" + username + "', '" + user.Password + "', '" + user.Salt + "');";
         
          
            using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
            {
                connection.Open();
                connection.Execute(sql);
            }
            return "success";
        }


        [HttpGet("check")]
        public void CheckUser(string email, string password)
        {
            User user = new User(email, password);

            // Сделать запрос к базе и узнать


            string sql = "SELECT 'Salt' FROM public.\"Users\" WHERE \"UserId\" = '54';";

            using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
            {
                connection.Open();
                //var customer = connection.Query<User>(sql, new { Username });

             /*  foreach(User c in customer)
                {
                    System.Diagnostics.Debug.WriteLine(c);
                }*/
                    

            }

            
        }


    }
}

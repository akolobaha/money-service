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
        public bool Index(string username, string password)
        {
            User user = new User(username, password);

            // Проверка email на валидность
            UserService checkUser = new UserService();
            if (!checkUser.IsValidEmail(username))
                return false;

            
            using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
            {
                string sql = "INSERT INTO public.\"Users\" (\"Username\", \"Password\", \"Salt\") " +
                    "VALUES('" + username + "', '" + user.Password + "', '" + user.Salt + "');";

                // Проверка логина на уникальность.
                string sqlIsUserUnique = $"SELECT * FROM \"Users\" WHERE \"Username\"='{username}';";
                connection.Open();
                var uniqueUser = connection.Query<UserDb>(sqlIsUserUnique).ToList();
                if (uniqueUser.Count() > 0)
                    return false;

                connection.Execute(sql);
            }
            return true;
        }


        [HttpGet("check")]
        public bool CheckUser(string username, string password)
        {
            //User user = new User(email, password);

            // connection.Query<Customer>("Select * FROM CUSTOMERS WHERE CustomerName = 'Mark'").ToList();
            string sql = $"SELECT * FROM \"Users\" WHERE \"Username\"=\'{username}\';";
            //System.Diagnostics.Debug.WriteLine(sql);

            using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
            {
                connection.Open();
                //var customer = connection.Query<User>(sql);
                //var customer = connection.QueryMultiple(sql).Read<string>().FirstOrDefault(); // прочитал первое значение строки
                //var customer = connection.QueryMultiple(sql).Read<string>();// прочитал 1е значине
                var usrDb = connection.Query<UserDb>(sql).ToList();
                System.Diagnostics.Debug.WriteLine(usrDb[0]);
                    
                UserService check = new UserService();
                var chk = check.IsValidUser(password, usrDb[0].Salt, usrDb[0].Password);
                System.Diagnostics.Debug.WriteLine(chk);

                return chk;
                /*customer[0].UserId;
                customer[0].Username;
                customer[0].Password;
                customer[0].ModerationCompleted;
                customer[0].Salt;*/


                /*  foreach(User c in customer)
                   {
                       System.Diagnostics.Debug.WriteLine(c);
                   }*/


            }

            
        }

        


    }
}

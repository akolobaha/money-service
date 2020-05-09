using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MoneyService.BuisnessLogic.Auth;
using MoneyService.DB;
using MoneyService.Model;
using Npgsql;

namespace MoneyService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
        }


        [Authorize]
        public string Index ()
        {
            // Вывести акаунты, которые есть у пользователя. Или показать, что аккаунтов пока нет
            //System.Diagnostics.Debug.WriteLine("df");
        
            return "userName";
        }

        [Authorize]
        [HttpPost("Create")]
        public string Create ()
        {
            string username = GetLoginByToken();
            int userId = GetUserIdByLogin(username);

            // Получить id из логина
         /*   using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();
                System.Diagnostics.Debug.WriteLine(users[0].Username);
                foreach (UserModel u in users)
                {
                    if (u.Username == userLogin)
                        userId = u.UserId;
                }
                // Создать запись в бд
                if (userId != 0)
                {
                    AccountModel account = new AccountModel { AccountOwnersId = userId };
                    db.Account.Add(account);
                    db.SaveChanges();

                    return "Successfuly created";
                }

            }*/

            //INSERT INTO "Account"("AccountOwnersId") VALUES(57);
            try
            {
                using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
                {
                    string sql = $"INSERT INTO \"Account\" (\"AccountOwnersId\") VALUES ({userId});";
                    connection.Open();
                    connection.Execute(sql);
                }
                return username + "|" + userId + "| счет успешно открыт";
            }
            
            catch
            {
                return "Ошибка добавления в базу";
            }





        }

        [Authorize]
        public string GetLoginByToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var username = claim[0].Value;
            return username;
        }
        public int GetUserIdByLogin(string username)
        {
            using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
            {
                string sql = $"SELECT * FROM \"Users\" WHERE \"Username\" = '{username}';";
                connection.Open();
                var user = connection.Query<UserDb>(sql).ToList();
                int userId = Int32.Parse(user[0].UserId);
                return userId; 
            }
        }



        [Authorize]
        [HttpGet("top-up-balance")]
        public string TopUpBalance(long accnum, int amount)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var username = claim[0].Value;

            int userId = 0;

            // Получить id из логина
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();
                foreach (UserModel u in users)
                {
                    if (u.Username == username)
                        userId = u.UserId;
                }

                if (userId != 0)
                {
                    var accounts = db.Account.ToList();
                    foreach(AccountModel a in accounts)
                    {
                        if (a.AccountNumber == accnum && a.AccountOwnersId == userId)
                        {
                            a.AccountBalance += amount;
                            db.SaveChanges();
                            return ($"Баланс счета {a.AccountNumber} пополнен на {amount}");

                        }
                            
                    }
                    
                }
            }

            return ("Ошибка. Баланс не был пополнен");
        }

        [Authorize]
        [HttpGet("transfer")]
        public string Transfer(long accnumA, long accnumB, int amount)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userLogin = claim[0].Value;

            int userId = 0;

            // Получить id из логина
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();
                foreach (UserModel u in users)
                {
                    if (u.Username == userLogin)
                        userId = u.UserId;
                }

                if (userId != 0)
                {
                    var accounts = db.Account.ToList();
                    foreach (AccountModel a in accounts)
                    {
                        if ((a.AccountNumber == accnumA && a.AccountOwnersId == userId) && a.AccountBalance >= amount)
                            foreach (AccountModel b in accounts)
                            {
                                if (b.AccountNumber == accnumB)
                                {
                                    a.AccountBalance -= amount;
                                    b.AccountBalance += amount;
                                    db.SaveChanges();
                                    return ("Перевод выполнен успешно");
                                }
                            }
                    }
                }
                else
                    return ("Пользователь не найден");
            }

            return ("Ошибка. Перевод не выполнен.");
        }

    }
}
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


        // Пользователь и его аккаунты
        [Authorize]
        [HttpGet]
        public string Index ()
        {
            using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
            {
                string username = GetLoginByToken();
                int userId = GetUserIdByLogin(username);

               
                string res = $" {{ \"username\" : \"{ username}\", \"accounts\" : {{ ";


                string sql = $"SELECT * FROM \"Account\" WHERE \"AccountOwnersId\" = {userId};";
                var accounts = connection.Query<AccountModel>(sql).ToList();
                foreach (AccountModel acc in accounts)
                {

                    res += $" \"{acc.AccountNumber}\" : {acc.AccountBalance},";
                }

                res += "}}";
                return res;
            }
        }

        [Authorize]
        [HttpPost("Create")]
        public bool Create ()
        {
            string username = GetLoginByToken();
            int userId = GetUserIdByLogin(username);

            try
            {
                using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
                {
                    string sql = $"INSERT INTO \"Account\" (\"AccountOwnersId\") VALUES ({userId});";
                    connection.Open();
                    connection.Execute(sql);
                }
                return true;
            }
            
            catch
            {
                return false;
            }
        }

        [Authorize]
        [HttpGet("refill")]
        public bool TopUpBalance(long accnum, int amount)
        {
            string username = GetLoginByToken();
            int userId = GetUserIdByLogin(username);
            AccountModel Account;


            try
            {
                using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
                {
                    // Получить текущий баланс
                    string sql1 = $"SELECT * FROM \"Account\" WHERE \"AccountOwnersId\" = {userId} AND \"AccountNumber\" = {accnum};";
                    var account = connection.Query<AccountModel>(sql1).ToList();
                    Account = account[0];
                    Account.AccountBalance += amount;


                    string sql2 = $"" +
                        $"UPDATE \"Account\" SET \"AccountBalance\" = {Account.AccountBalance} " +
                        $"WHERE \"AccountOwnersId\" = {userId} AND \"AccountNumber\" = {accnum};";
                    connection.Execute(sql2);

                }
                return true;
            }

            catch
            {
                return false;
            }

        }

        [Authorize]
        [HttpGet("transfer")]
        public bool Transfer(long accnumA, long accnumB, int amount)
        {
            string username = GetLoginByToken();
            int userId = GetUserIdByLogin(username);
            AccountModel AccountA, AccountB;

            // Проверка на отрицательную сумму, совпадение аккаунтов
            if (amount < 0 || accnumA == accnumB)
                return false;

            try
            {
                using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
                {
                    // Получить аккаунтА
                    string sql1 =
                        $"SELECT * FROM \"Account\" " +
                        $"WHERE \"AccountOwnersId\" = {userId} AND \"AccountNumber\" = {accnumA};";
                    var accounta = connection.Query<AccountModel>(sql1).ToList();
                    AccountA = accounta[0];

                    // Достаточна ли сумма?
                    if (AccountA.AccountBalance < amount)
                        return false;

                    // Получить баланс аккаунта Б
                    string sql2 = $"SELECT * FROM \"Account\" " +
                                    $"WHERE \"AccountNumber\" = {accnumB};";
                    var accountb = connection.Query<AccountModel>(sql1).ToList();
                    AccountB = accountb[0];

                    // Прибавить к аккаунта Б необходимую сумму
                    AccountB.AccountBalance += amount;
                    string sql3 = $"" +
                            $"UPDATE \"Account\" SET \"AccountBalance\" = {AccountB.AccountBalance} " +
                            $"WHERE \"AccountNumber\" = {accnumB};";
                    connection.Execute(sql3);

                    // Снять с аккаунта А переведенную сумму
                    AccountA.AccountBalance -= amount;
                    string sql4 = $"" +
                            $"UPDATE \"Account\" SET \"AccountBalance\" = {AccountA.AccountBalance} " +
                            $"WHERE \"AccountNumber\" = {accnumA};";
                    connection.Execute(sql4);
                }
                return true;
            }
            
            catch
            {
                return false;
            }

        }

        private string GetLoginByToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var username = claim[0].Value;
            return username;
        }
        private int GetUserIdByLogin(string username)
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

    }
}
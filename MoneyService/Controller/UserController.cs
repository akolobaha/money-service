﻿using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoneyService.BuisnessLogic.Auth;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MoneyService.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("login")]
        public IActionResult Login(string username, string password)
        {
            IActionResult response = Unauthorized();

            var ValidUser = AuthenticationUser(username, password);

            if (ValidUser)
            {
                User user = new User(username, password);
                var tokenStr = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenStr });
            }

            return response;
        }

        [HttpGet("registration")]
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

        // Проверяем пользователя
        private bool AuthenticationUser(string username, string password)
        {
            string sql = $"SELECT * FROM \"Users\" WHERE \"Username\"=\'{username}\';";

            try
            {
                using (var connection = new NpgsqlConnection(_config["ConnectionStrings:Users"]))
                {
                    connection.Open();
                    var usrDb = connection.Query<UserDb>(sql).ToList();

                    UserService check = new UserService();
                    var chk = check.IsValidUser(password, usrDb[0].Salt, usrDb[0].Password);

                    return chk;
                }
            }
            catch
            {
                return false;
            }
        }

        // Генерируем токен
        private string GenerateJSONWebToken(User userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userinfo.Username),
                /*new Claim(JwtRegisteredClaimNames.Email,userinfo.EmailAddress),*/
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        // Кто я?
        [Authorize]
        [HttpPost("Post")]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userName = claim[0].Value;
            return "Welcome To: " + userName;
        }

        [Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Value1", "Value2", "Value3" };
        }
    }
}
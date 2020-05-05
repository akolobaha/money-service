using Microsoft.AspNetCore.Mvc;
using MoneyService.DB;
using MoneyService.Model;
using System;
using System.Linq;

namespace MoneyService.Controller
{
    [Route("[controller]")]
    public  class RegisterController : ControllerBase
    {
        /*public string username;
        public string password;*/
        /*   [HttpGet]
           public void Register(string username, string password)
           {
               this.username = username;
               this.password = password;
           }*/
        [HttpGet]
        public string Index(string email, string password)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // Создаем два объекта User

                UserModel user1 = new UserModel { EmailAddress = email, Password= password, ModerationCompleted = false };

                // Добавляем объекты в БД
                if (user1.EmailAddress != null && user1.Password != null)
                {
                    db.Users.Add(user1);
                    db.SaveChanges();
                }
                

                // Получаем объекты из бд и выводим на консоль
                /*var users = db.Users.ToList();*/
                /* foreach (UserModel u in users)
                 {
                     Console.WriteLine($"{u.UserId}.{u.UserName}");
                 }*/
            }
            return "Email: " + email + " Password " + password;
        }


        
    }
}

using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using BoxrecScraper.Models;

namespace BoxrecScraper.Utils   
{
    public static class LoginProvider
    {
        private static readonly Random _rnd = new Random();
        public static (string email, string password) GetLoginCredentials()
        {
            LoginCredentials[] loginData =
            [
                new LoginCredentials { email = "yourgmail@gmail.com", password = "yourpassword" },
            ];
            int index = _rnd.Next(loginData.Length);
            return (loginData[index].email, loginData[index].password);
        }

    }
}

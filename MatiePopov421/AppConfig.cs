using MatiePopov421.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MatiePopov421
{
    public static class AppConfig
    {
        public const string ConnectionString =
            "Host=localhost;Port=5432;Database=matiedb;Username=postgres;Password=root";

        public static User? CurrentUser { get; set; }

        public static bool IsModerator =>
            CurrentUser?.Role?.Name == "Модератор";

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}

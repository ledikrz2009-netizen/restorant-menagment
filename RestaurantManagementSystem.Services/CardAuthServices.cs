using System;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Services
{
    public interface ICardLoginService
    {
        string ReadCardUid();
    }

    public class MockCardLoginService : ICardLoginService
    {
        public string ReadCardUid()
        {
            return "MOCK-WAITER-CARD-001";
        }
    }

    public interface IPasswordHasher
    {
        string Hash(string plainText);
        bool Verify(string plainText, string hash);
    }

    public class BasicPasswordHasher : IPasswordHasher
    {
        public string Hash(string plainText)
        {
            if (plainText == null) return null;
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));
        }

        public bool Verify(string plainText, string hash)
        {
            return string.Equals(Hash(plainText), hash, StringComparison.Ordinal);
        }
    }
}

using DAL.Models;

namespace WinFormMain.Helpers
{
    public static class UserHelper
    {
        private static readonly Random _random = new();

        public static User NewUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = $"John Doe {_random.Next(100, 999)}",
                Email = $"johndoe{_random.Next(100, 999)}@example.com",
                Password = "Password123!",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }
}
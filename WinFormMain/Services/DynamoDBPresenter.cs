using DAL.Models;
using DAL.Services;

namespace WinFormMain.Services
{
    public class DynamoDBPresenter
    {
        private readonly IDynamoDBService _dynamoDBService;
        private readonly Random _random = new();
        private readonly Action<string> _logAction;

        public List<User> LocalUsers { get; private set; } = new();

        public DynamoDBPresenter(IDynamoDBService dynamoDBService, Action<string> logAction)
        {
            _dynamoDBService = dynamoDBService ?? throw new ArgumentNullException(nameof(dynamoDBService));
            _logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
        }

        public async Task AddUserAsync()
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "John Doe " + _random.Next(100, 999),
                Email = $"johndoe{_random.Next(100, 999)}@example.com",
                Password = "Password123!",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _dynamoDBService.AddAsync(newUser);
            LocalUsers.Add(newUser);
            _logAction($"[SUCCESS] Added user {newUser.Id} (Name: {newUser.Name}) to local DynamoDB.");
        }

        public async Task UpdateUserAsync(Guid userId)
        {
            var userToUpdate = LocalUsers.FirstOrDefault(u => u.Id == userId);
            if (userToUpdate != null)
            {
                userToUpdate.Name = "Updated Name " + _random.Next(10, 99);
                userToUpdate.UpdatedAt = DateTime.Now;
                await _dynamoDBService.UpdateAsync(userToUpdate);
                _logAction($"[SUCCESS] Updated user {userToUpdate.Id} to name: {userToUpdate.Name}");
            }
            else
            {
                _logAction($"[ERROR] Selected user {userId} not found locally.");
            }
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _dynamoDBService.DeleteAsync(userId);
            var userToRemove = LocalUsers.FirstOrDefault(u => u.Id == userId);
            if (userToRemove != null)
            {
                LocalUsers.Remove(userToRemove);
            }
            _logAction($"[SUCCESS] Deleted user {userId}");
        }

        public async Task SearchUsersAsync()
        {
            var searchResults = await _dynamoDBService.SearchAsync<User>("Name", "Name");
            LocalUsers = searchResults.ToList();
            _logAction($"[SUCCESS] Search returned {LocalUsers.Count} user(s) matching Name='Name'.");
            foreach (var user in LocalUsers)
            {
                _logAction($"  - ID: {user.Id}, Name: {user.Name}, Email: {user.Email}");
            }
        }

        public async Task<List<User>> LoadAllUsersAsync()
        {
            var results = await _dynamoDBService.SearchAsync<User>("", "");
            LocalUsers = results.ToList();
            return LocalUsers;
        }

        public void ClearUsers()
        {
            LocalUsers.Clear();
        }
    }
}

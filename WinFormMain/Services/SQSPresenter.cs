using DAL.Models;
using SQS.Models;
using SQS.Services;
using WinFormMain.Helpers;
using static Base.Models.Constants;

namespace WinFormMain.Services
{
    public class SQSPresenter
    {
        private readonly IUserQueueService _sqsService;
        private readonly Random _random = new();
        private readonly Action<string> _logAction;

        public List<User> LocalUsers { get; private set; } = new();

        public SQSPresenter(IUserQueueService sqsService, Action<string> logAction)
        {
            _sqsService = sqsService ?? throw new ArgumentNullException(nameof(sqsService));
            _logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
        }

        public async Task AddUserAsync()
        {
            var message = new SQSMessage()
            {
                Action = UserActions.Add,
                User = UserHelper.NewUser()
            };

            await _sqsService.SendMessageAsync(message);
            LocalUsers.Add(message.User);
            _logAction($"[SUCCESS] Added user {message.User.Id} (Name: {message.User.Name}) to local DynamoDB.");
        }

        public async Task UpdateUserAsync(Guid userId)
        {
            var userToUpdate = LocalUsers.FirstOrDefault(u => u.Id == userId);
            if (userToUpdate != null)
            {
                userToUpdate.Name = "Updated Name " + _random.Next(10, 99);
                userToUpdate.UpdatedAt = DateTime.Now;
                var message = new SQSMessage()
                {
                    Action = UserActions.Update,
                    User = userToUpdate
                };

                await _sqsService.SendMessageAsync(message);
                _logAction($"[SUCCESS] Updated user {userToUpdate.Id} to name: {userToUpdate.Name}");
            }
            else
            {
                _logAction($"[ERROR] Selected user {userId} not found locally.");
            }
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var message = new SQSMessage()
            {
                Action = UserActions.Delete,
                User = new User() { Id = userId }
            };
            await _sqsService.SendMessageAsync(message);
            var userToRemove = LocalUsers.FirstOrDefault(u => u.Id == userId);
            if (userToRemove != null)
            {
                LocalUsers.Remove(userToRemove);
            }
            _logAction($"[SUCCESS] Deleted user {userId}");
        }

        public async Task SearchUsersAsync()
        {
            var message = new SQSMessage() { Action = UserActions.Search, User = new User() { Name = "Name" } };
            var searchResults = await _sqsService.SendMessageAsync(message);
            // LocalUsers = searchResults.ToList();
            _logAction($"[SUCCESS] Search returned {LocalUsers.Count} user(s) matching Name='Name'.");
            foreach (var user in LocalUsers)
            {
                _logAction($"  - ID: {user.Id}, Name: {user.Name}, Email: {user.Email}");
            }
        }

        public async Task<List<User>> LoadAllUsersAsync()
        {
            var results = await _sqsService.SendMessageAsync(new SQSMessage() { Action = UserActions.Search, User = new User() { Name = "" } });
            // LocalUsers = results.ToList();
            return LocalUsers;
        }

        public void ClearUsers()
        {
            LocalUsers.Clear();
        }
    }
}

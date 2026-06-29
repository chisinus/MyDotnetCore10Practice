using WinFormMain.Models;
using WinFormMain.Services;
using static WinFormMain.Models.Constants;

namespace WinFormMain
{
    public partial class MainForm : Form
    {
        private readonly IDynamoDBService _dynamoDBService;
        private readonly Random _random = new();
        private List<User> localUsers = new();

        public MainForm(IDynamoDBService dynamoDBService)
        {
            InitializeComponent();
            _dynamoDBService = dynamoDBService;

            // Set the default selected index for the action combobox to Add
            if (cbDynamoDBActions.Items.Count > 0)
            {
                cbDynamoDBActions.SelectedIndex = 0;
            }
        }

        private async void btnDynamoDBGo_Click(object sender, EventArgs e)
        {
            if (cbDynamoDBActions.SelectedIndex == -1)
            {
                LogResult("Please select a DynamoDB action first.");
                return;
            }

            var action = (DynamoDBActions)cbDynamoDBActions.SelectedIndex;
            LogResult($"Executing DynamoDB Action: {action}...");

            try
            {
                switch (action)
                {
                    case DynamoDBActions.Add:
                        var newUser = new User
                        {
                            Id = Guid.NewGuid(),
                            Name = "John Doe " + _random.Next(100, 999),
                            Email = $"johndoe{_random.Next(100, 999)}@example.com",
                            Password = "Password123!",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        await _dynamoDBService.AddAsync("tblUsers", newUser);
                        localUsers.Add(newUser);
                        LogResult($"[SUCCESS] Added user {newUser.Id} (Name: {newUser.Name}) to local DynamoDB.");
                        break;

                    case DynamoDBActions.Update:
                        if (cbUsers.SelectedIndex != -1 && Guid.TryParse(cbUsers.SelectedItem?.ToString(), out Guid updateId))
                        {
                            var userToUpdate = localUsers.FirstOrDefault(u => u.Id == updateId);
                            if (userToUpdate != null)
                            {
                                userToUpdate.Name = "Updated Name " + _random.Next(10, 99);
                                userToUpdate.UpdatedAt = DateTime.Now;
                                await _dynamoDBService.UpdateAsync("tblUsers", userToUpdate);
                                LogResult($"[SUCCESS] Updated user {userToUpdate.Id} to name: {userToUpdate.Name}");
                            }
                            else
                            {
                                LogResult($"[ERROR] Selected user {updateId} not found locally.");
                            }
                        }
                        else
                        {
                            LogResult("No user selected or loaded for update. Select Update/Delete to load users.");
                        }
                        break;

                    case DynamoDBActions.Delete:
                        if (cbUsers.SelectedIndex != -1 && Guid.TryParse(cbUsers.SelectedItem?.ToString(), out Guid deleteId))
                        {
                            await _dynamoDBService.DeleteAsync("tblUsers", deleteId);
                            var userToRemove = localUsers.FirstOrDefault(u => u.Id == deleteId);
                            if (userToRemove != null)
                            {
                                localUsers.Remove(userToRemove);
                            }
                            
                            cbUsers.Items.RemoveAt(cbUsers.SelectedIndex);
                            if (cbUsers.Items.Count > 0)
                            {
                                cbUsers.SelectedIndex = 0;
                            }
                            else
                            {
                                cbUsers.SelectedIndex = -1;
                            }
                            LogResult($"[SUCCESS] Deleted user {deleteId}");
                        }
                        else
                        {
                            LogResult("No user selected or loaded for deletion. Select Update/Delete to load users.");
                        }
                        break;

                    case DynamoDBActions.Search:
                        var searchResults = await _dynamoDBService.SearchAsync<User>("tblUsers", "Name", "Name");
                        localUsers = searchResults.ToList();
                        LogResult($"[SUCCESS] Search returned {localUsers.Count} user(s) matching Name='Name'.");
                        foreach (var user in localUsers)
                        {
                            LogResult($"  - ID: {user.Id}, Name: {user.Name}, Email: {user.Email}");
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                LogResult($"Error executing action: {ex.Message}");
            }
        }

        private void LogResult(string message)
        {
            richTextBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            richTextBox1.ScrollToCaret();
        }

        private async void cbDynamoDBActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDynamoDBActions.SelectedIndex == -1)
            {
                LogResult("Please select a DynamoDB action first.");
                return;
            }

            DynamoDBActions action = (DynamoDBActions)cbDynamoDBActions.SelectedIndex;
            try
            {
                cbUsers.Items.Clear();
                if (action == DynamoDBActions.Update || action == DynamoDBActions.Delete)
                {
                    var results = await _dynamoDBService.SearchAsync<User>("tblUsers", "", "");
                    localUsers = results.ToList();
                    LogResult($"Loaded {localUsers.Count} users for Update/Delete.");

                    foreach (var user in localUsers)
                    {
                        cbUsers.Items.Add(user.Id.ToString());
                    }
                    if (cbUsers.Items.Count > 0)
                    {
                        cbUsers.SelectedIndex = 0;
                    }
                    cbUsers.Enabled = true;
                    btnDynamoDBGo.Enabled = cbUsers.Items.Count > 0;
                }
                else if (action == DynamoDBActions.Search)
                {
                    //var results = await _dynamoDBService.SearchAsync<User>("tblUsers", "Name", "Name");
                    //localUsers = results.ToList();
                    cbUsers.Enabled = false;
                    btnDynamoDBGo.Enabled = true;
                }
                else
                {
                    cbUsers.Enabled = false;
                    localUsers.Clear();
                    btnDynamoDBGo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogResult($"Error loading users for index change: {ex.Message}");
            }
        }
    }
}

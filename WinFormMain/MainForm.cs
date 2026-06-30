using DAL.Services;
using SQS.Services;
using WinFormMain.Services;
using static Base.Models.Constants;

namespace WinFormMain
{
    public partial class MainForm : Form
    {
        private readonly DynamoDBPresenter _presenter;
        private readonly SQSPresenter _sqsPresenter;

        public MainForm(IDynamoDBService dynamoDBService, IUserQueueService userQueueService)
        {
            InitializeComponent();
            _presenter = new DynamoDBPresenter(dynamoDBService, LogResult);
            _sqsPresenter = new SQSPresenter(userQueueService, LogResult);

            // Set the default selected index for the action combobox to Add
            if (cbUserActions.Items.Count > 0)
            {
                cbUserActions.SelectedIndex = 0;
            }
        }

        private void LogResult(string message)
        {
            richTextBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            richTextBox1.ScrollToCaret();
        }

        #region Common event handler
        private async void cbUserActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbUserActions.SelectedIndex == -1)
            {
                LogResult("Please select a DynamoDB action first.");
                return;
            }

            UserActions action = (UserActions)cbUserActions.SelectedIndex;
            try
            {
                cbUsers.Items.Clear();
                if (action == UserActions.Update || action == UserActions.Delete)
                {
                    var users = await _presenter.LoadAllUsersAsync();
                    LogResult($"Loaded {users.Count} users for Update/Delete.");

                    foreach (var user in users)
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
                else if (action == UserActions.Search)
                {
                    cbUsers.Enabled = false;
                    btnDynamoDBGo.Enabled = true;
                }
                else
                {
                    cbUsers.Enabled = false;
                    _presenter.ClearUsers();
                    btnDynamoDBGo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogResult($"Error loading users for index change: {ex.Message}");
            }
        }
        #endregion

        #region Dynamodb event handler
        private async void btnDynamoDBGo_Click(object sender, EventArgs e)
        {
            if (cbUserActions.SelectedIndex == -1)
            {
                LogResult("Please select a DynamoDB action first.");
                return;
            }

            var action = (UserActions)cbUserActions.SelectedIndex;
            LogResult($"Executing DynamoDB Action: {action}...");

            try
            {
                switch (action)
                {
                    case UserActions.Add:
                        await _presenter.AddUserAsync();
                        break;

                    case UserActions.Update:
                        if (cbUsers.SelectedIndex != -1 && Guid.TryParse(cbUsers.SelectedItem?.ToString(), out Guid updateId))
                        {
                            await _presenter.UpdateUserAsync(updateId);
                        }
                        else
                        {
                            LogResult("No user selected or loaded for update. Select Update/Delete to load users.");
                        }
                        break;

                    case UserActions.Delete:
                        if (cbUsers.SelectedIndex != -1 && Guid.TryParse(cbUsers.SelectedItem?.ToString(), out Guid deleteId))
                        {
                            await _presenter.DeleteUserAsync(deleteId);

                            cbUsers.Items.RemoveAt(cbUsers.SelectedIndex);
                            if (cbUsers.Items.Count > 0)
                            {
                                cbUsers.SelectedIndex = 0;
                            }
                            else
                            {
                                cbUsers.SelectedIndex = -1;
                            }
                        }
                        else
                        {
                            LogResult("No user selected or loaded for deletion. Select Update/Delete to load users.");
                        }
                        break;

                    case UserActions.Search:
                        await _presenter.SearchUsersAsync();
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


        #endregion

        #region SQS event handler
        private async void btnSQSSubmit_Click(object sender, EventArgs e)
        {
            var action = (UserActions)cbUserActions.SelectedIndex;
            LogResult($"Executing User Action: {action}...");

            try
            {
                switch (action)
                {
                    case UserActions.Add:
                        await _sqsPresenter.AddUserAsync();
                        break;

                    case UserActions.Update:
                        if (cbUsers.SelectedIndex != -1 && Guid.TryParse(cbUsers.SelectedItem?.ToString(), out Guid updateId))
                        {
                            await _sqsPresenter.UpdateUserAsync(updateId);
                        }
                        else
                        {
                            LogResult("No user selected or loaded for update. Select Update/Delete to load users.");
                        }
                        break;

                    case UserActions.Delete:
                        if (cbUsers.SelectedIndex != -1 && Guid.TryParse(cbUsers.SelectedItem?.ToString(), out Guid deleteId))
                        {
                            await _sqsPresenter.DeleteUserAsync(deleteId);

                            cbUsers.Items.RemoveAt(cbUsers.SelectedIndex);
                            if (cbUsers.Items.Count > 0)
                            {
                                cbUsers.SelectedIndex = 0;
                            }
                            else
                            {
                                cbUsers.SelectedIndex = -1;
                            }
                        }
                        else
                        {
                            LogResult("No user selected or loaded for deletion. Select Update/Delete to load users.");
                        }
                        break;

                    case UserActions.Search:
                        await _sqsPresenter.SearchUsersAsync();
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
        #endregion
    }
}

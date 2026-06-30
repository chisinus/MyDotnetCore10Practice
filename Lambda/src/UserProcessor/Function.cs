using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using DAL.Models;
using DAL.Services;
using SQS.Models;
using Base.Models;
using static Base.Models.Constants;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda
{
    public class Function
    {
        private readonly DynamoDBService _repo = new DynamoDBService();

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<SQSMessage>(record.Body);
                    context.Logger.LogLine($">>>>>>>>>>>>>>>>>>Processing record. Message ID: {record.MessageId}. Body: {record.Body}");

                    if (data == null) continue;

                    switch ((Constants.UserActions)data.Action)
                    {
                        case UserActions.Add:
                            await _repo.AddAsync(data.User);
                            break;
                        case UserActions.Update:
                            await _repo.UpdateAsync(data.User);
                            break;
                        case UserActions.Delete:
                            await _repo.DeleteAsync(data.User.Id);
                            break;
                        case UserActions.Search:
                            await _repo.SearchAsync<User>("Name", data.User.Name);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine($">>>>>>>>>>>>>>>> Error processing record {record.MessageId}: {ex.Message}. StackTrace: {ex.StackTrace}");
                }
            }
        }
    }
}

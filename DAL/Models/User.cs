using System;
using Amazon.DynamoDBv2.DataModel;

namespace DAL.Models
{
    [DynamoDBTable("tblUsers")]
    public class User
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }
        [DynamoDBProperty]
        public string Name { get; set; } = string.Empty;
        [DynamoDBProperty]
        public string Email { get; set; } = string.Empty;
        [DynamoDBProperty]
        public string Password { get; set; } = string.Empty;
        [DynamoDBProperty]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DynamoDBProperty]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

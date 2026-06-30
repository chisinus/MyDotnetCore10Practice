using Base.Models;
using DAL.Models;

namespace SQS.Models
{
    public class SQSMessage
    {
        public required Constants.UserActions Action { get; set; }
        public required User User { get; set; }
    }
}
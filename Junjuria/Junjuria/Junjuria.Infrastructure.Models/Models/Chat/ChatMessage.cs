using System.ComponentModel.DataAnnotations.Schema;

namespace Junjuria.Infrastructure.Models.Models.Chat
{
    public class ChatMessage : BaseEntity<int>
    {
    public string OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public virtual AppUser Owner { get; set; }

        public string SenderName { get; set; } //owner Or not!
        public string Message { get; set; }

        public string SenderRole { get; set; } //owner Or not!
    }
}



//public class ResponseMessage
//{
//    public ResponseMessage() { }

//    public ResponseMessage(string userName, string userRole, string message, string targetName = null)
//    {
//        Message = message;
//        TargetName = targetName;
//        Date = GlobalConstants.ServerTimeConvert(DateTime.UtcNow.ToLocalTime());
//        UserInfo = new UserInfo(userRole, userName);
//    }

//    public string TargetName { get; set; }
//    public UserInfo UserInfo { get; set; }
//    public string Message { get; set; }
//    public string Date { get; set; }
//}
//public class UserInfo
//{
//    public UserInfo() { }
//    public UserInfo(string role, string userName)
//    {
//        Role = role;
//        UserName = userName;
//    }
//    public string Role { get; set; }
//    public string UserName { get; set; }
//}
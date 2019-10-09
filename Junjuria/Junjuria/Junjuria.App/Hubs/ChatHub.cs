namespace Junjuria.App.Hubs
{
    using Junjuria.Common;
    using Junjuria.Infrastructure.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Authorize]
    public class ChatHub : Hub
    {
        private const string serviceRoom = "staff";
        private readonly UserManager<AppUser> userManager;
        private IList<string> usersSeekingAsistance;
      
        public ChatHub(UserManager<AppUser> userManager)
        {
            usersSeekingAsistance = new List<string>();
            this.userManager = userManager;
        }

        [Authorize(Roles = "Admin,Assistance")]
        public async Task JoinServiceStaff() =>
               await Groups.AddToGroupAsync(this.Context.ConnectionId, serviceRoom);

        [Authorize(Roles = "Admin")]
        public async Task Broadcast(string message) =>
               await Clients.All.SendAsync("ReceiveMassMessage", GetResponse(message));

        public async Task UserMessaging(string message) =>
               await Clients.Groups(serviceRoom).SendAsync("ReceiveMessage", GetResponse(message));

        public async Task NewCommer()
        {
            if (Context.User.IsInRole("Admin") || Context.User.IsInRole("Assistance"))
            {    //populate chat tabs with all users seeking guidance :)
                foreach (var user in usersSeekingAsistance)
                {
                    await Clients.Caller.SendAsync("PopChatTab", user);
                }
            }
            else
            {   //pop new chat-tab in service staff members who are currently watching!
                var user = await userManager.GetUserAsync(Context.User);
                usersSeekingAsistance.Add(user.Id);
                await Clients.Group(serviceRoom).SendAsync("PopChatTab", user);
            }
        }

        public async Task StaffMessagingUser(string message, string userId)
        {
            await Clients.User(userId)//sending message to the user
                         .SendAsync("ReceiveMessage", GetResponse(message));
            await Clients.Group(serviceRoom)//sending message for other staff to see/not to repeat answers
                         .SendAsync("ReceiveMessage", GetResponse(message));
        }

        private ResponseMessage GetResponse(string message)
        {
            var userRole = Context.User.IsInRole("Admin") ? "Admin" : Context.User.IsInRole("Assistance") ? "Assistance" : "User";
            return new ResponseMessage(Context.User.Identity.Name, userRole, message);
        }
    }

    public class ResponseMessage
    {
        public ResponseMessage(string userName, string userRole, string message)
        {
            Message = message;
            Date = GlobalConstants.TimeFormatAccepted(DateTime.UtcNow.ToLocalTime());
            UserInfo = new UserInfo( userRole, userName);
        }

        public UserInfo UserInfo { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }
    public class UserInfo
    {
        public UserInfo(string role, string userName)
        {
            Role = role;
            UserName = userName;
        }
        public string Role { get; set; }
        public string UserName { get; set; }
    }
}
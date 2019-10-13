namespace Junjuria.App.Hubs
{
    using Junjuria.Common;
    using Junjuria.Infrastructure.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    public class ChatHub : Hub
    {
        private const string serviceRoom = "staff";
        private readonly UserManager<AppUser> userManager;
        private static IList<string> usersSeekingAsistance;
        private static IList<string> staffProvidingAsistance;

        static ChatHub()
        {
            usersSeekingAsistance = new List<string>();
            staffProvidingAsistance = new List<string>();
        }
        public ChatHub(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task NewCommer()
        {
            string userName = Context.User.Identity.Name;
            if (Context.User.IsInRole("Admin") || Context.User.IsInRole("Assistance"))
            {    //populate chat tabs with all users seeking guidance :)
                //if (usersSeekingAsistance.Contains(userName)) return; //TODO IS it needed?
                foreach (var member in usersSeekingAsistance)
                {
                    await Clients.Caller.SendAsync("PopChatTab", member);
                    await Clients.Caller.SendAsync("AddUserNameToStaffPanel", member);
                }
                foreach (var member in staffProvidingAsistance.Where(x => x != userName))
                {
                    await Clients.Caller.SendAsync("AddStaffNameToStaffPanel", member);
                }
                if (!staffProvidingAsistance.Contains(userName))
                {
                    staffProvidingAsistance.Add(userName);
                    await Clients.Group(serviceRoom).SendAsync("AddStaffNameToStaffPanel", userName);//pop new staffMember to other staff members
                }
                await JoinServiceStaff();
            }
            else
            {   //pop new chat-tab in service staff members who are currently watching!
                if (usersSeekingAsistance.Contains(userName)) return;
                usersSeekingAsistance.Add(Context.User.Identity.Name);
                await Clients.Group(serviceRoom).SendAsync("PopChatTab", userName);
                await Clients.Group(serviceRoom).SendAsync("AddUserNameToStaffPanel", userName);
            }
        }

        [Authorize(Roles = "Admin,Assistance")]
        private async Task JoinServiceStaff()
        {
            string staffName = this.Context.User.Identity.Name;
            var connectionId = this.Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, serviceRoom, System.Threading.CancellationToken.None);
        }

        [Authorize(Roles = "Admin")]
        public async Task Broadcast(string message, bool sendToUsers = false, bool sendToStaff = false)
        {
            if (sendToUsers)
            {
                foreach (var member in usersSeekingAsistance)
                {
                    await StaffMessagingUser(message, member);
                }
            }
            if (sendToStaff)
            {
                var comment = GetResponse(message);
                await Clients.Group(serviceRoom).SendAsync("AdminInstructStaff", comment);
            }
        }




        public async Task UserMessaging(string message)
        {
            var comment = GetResponse(message);
            await Clients.Group(serviceRoom).SendAsync("ReceiveMessageA", comment);
            await Clients.Caller.SendAsync("ReceiveMessageU", comment);
        }

        public async Task StaffMessagingUser(string message, string targetName)
        {
            var userId = await userManager.Users.Where(x => x.UserName == targetName).Select(x => x.Id).FirstOrDefaultAsync();
            var comment = GetResponse(message, targetName);
            await Clients.User(userId).SendAsync("ReceiveMessageU", comment);//sending message to the user                    
            await Clients.Group(serviceRoom).SendAsync("ReceiveMessageA", comment);//sending message for other staff to see/not to repeat answers           
        }

        private ResponseMessage GetResponse(string message, string targetName = null)
        {
            //TargetName is provided when admin writes and needs to receive his own message in "target's tab!"
            //TargetName when user writes is not required because userName of sender and chat tab in userPanel is same.
            var userRole = Context.User.IsInRole("Admin") ? "Admin" : Context.User.IsInRole("Assistance") ? "Assistance" : "User";
            return new ResponseMessage(Context.User.Identity.Name, userRole, message, targetName);
        }
    }

    public class ResponseMessage
    {
        public string TargetName { get; set; }
        public ResponseMessage() { }

        public ResponseMessage(string userName, string userRole, string message, string targetName = null)
        {
            Message = message;
            TargetName = targetName;
            Date = GlobalConstants.TimeFormatAccepted(DateTime.UtcNow.ToLocalTime());
            UserInfo = new UserInfo(userRole, userName);
        }

        public UserInfo UserInfo { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }
    public class UserInfo
    {
        public UserInfo() { }
        public UserInfo(string role, string userName)
        {
            Role = role;
            UserName = userName;
        }
        public string Role { get; set; }
        public string UserName { get; set; }
    }
}
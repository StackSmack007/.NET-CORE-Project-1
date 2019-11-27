namespace Junjuria.App.Hubs
{
    using Junjuria.Common;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Models.Chat;
    using Junjuria.Services.Services.Contracts;
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
        private readonly IRepository<ChatMessage> messageRepository;
        private static IList<string> usersSeekingAsistance;
        private static IList<string> staffProvidingAsistance;

        static ChatHub()
        {
            usersSeekingAsistance = new List<string>();
            staffProvidingAsistance = new List<string>();
        }
        public ChatHub(UserManager<AppUser> userManager, IRepository<ChatMessage> messageRepository)
        {
            this.userManager = userManager;
            this.messageRepository = messageRepository;
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
                    var history = await GetChatHistory(member);
                    foreach (ResponseMessage message in history)
                    {
                        await Clients.Caller.SendAsync("ReceiveMessageA", message);
                    }
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
                if (usersSeekingAsistance.Contains(userName))
                {
                    return;
                    //todo refresh db records if any
                }
                string currentUserName = Context.User.Identity.Name;
                usersSeekingAsistance.Add(currentUserName);
                await Clients.Group(serviceRoom).SendAsync("AddUserNameToStaffPanel", userName);
                await Clients.Group(serviceRoom).SendAsync("PopChatTab", userName);

                var history = await GetChatHistory(currentUserName);
                foreach (ResponseMessage message in history)
                {
                    await Clients.Group(serviceRoom).SendAsync("ReceiveMessageA", message);
                    await Clients.Caller.SendAsync("ReceiveMessageU", message);
                }
            }
        }

        private async Task<ResponseMessage[]> GetChatHistory(string member)
        {
            string ownerId = userManager.Users.FirstOrDefault(x => x.UserName == member).Id;
            var messages = await messageRepository.All().Where(x => x.OwnerId == ownerId).Select(x => new ResponseMessage
            {
                TargetName = member,
                Message = x.Message,
                Date = GlobalConstants.ServerTimeConvert(x.DateOfCreation.ToLocalTime()),
                UserInfo = new UserInfo
                {
                    UserName = x.SenderName,
                    Role = x.SenderRole,
                }
            }).ToArrayAsync();

            return messages;
        }

        [Authorize(Roles = "User")]
        public async Task MemberExit()
        {
            string userName = Context.User.Identity.Name;
            if (usersSeekingAsistance.Contains(userName))
            {
                usersSeekingAsistance.Remove(userName);//Todo
                await Clients.Group(serviceRoom).SendAsync("unPopUserChatInStaffWindows", userName);
            }
        }

        [Authorize(Roles = "Admin,Assistance")]
        private async Task JoinServiceStaff()
        {
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
            string currentUsername = Context.User.Identity.Name;
            await StoreMessage(currentUsername, message, currentUsername);
        }

        public async Task StaffMessagingUser(string message, string targetName)
        {
            var userId = await userManager.Users.Where(x => x.UserName == targetName).Select(x => x.Id).FirstOrDefaultAsync();
            var comment = GetResponse(message, targetName);
            await Clients.User(userId).SendAsync("ReceiveMessageU", comment);//sending message to the author from himself 
            string currentUsername = Context.User.Identity.Name;
            await StoreMessage(targetName, message, currentUsername);
            await Clients.Group(serviceRoom).SendAsync("ReceiveMessageA", comment);//sending message for other staff to see/not to repeat answers           
        }

        private async Task StoreMessage(string ownerName, string message, string sender)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == ownerName);
            await messageRepository.AddAssync(new ChatMessage
            {
                Owner = user,
                SenderName = sender,
                SenderRole = Context.User.IsInRole("Admin") ? "Admin" : Context.User.IsInRole("Assistance") ? "Assistance" : "User",
                Message = message
            });

            var whatHappens = messageRepository.SaveChangesAsync().GetAwaiter().GetResult();



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
        public ResponseMessage() { }

        public ResponseMessage(string userName, string userRole, string message, string targetName = null)
        {
            Message = message;
            TargetName = targetName;
            Date = GlobalConstants.ServerTimeConvert(DateTime.UtcNow.ToLocalTime());
            UserInfo = new UserInfo(userRole, userName);
        }

        public string TargetName { get; set; }
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
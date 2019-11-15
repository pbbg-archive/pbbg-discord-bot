using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GoTCBot.Access.Abstractions;
using GoTCBot.Models;
using GoTCBot.Preconditions;

namespace GoTCBot.Modules
{
    public class HouseModule : ModuleBase<SocketCommandContext>
    {
        private IHouseAccess houseAccess;
        private IPlayerAccess userAccess; 
        public HouseModule(
            IHouseAccess houseAccess,
            IPlayerAccess playerAccess
        )
        {
            this.houseAccess = houseAccess;
            this.userAccess = playerAccess;
        }

        [Command("house_info")]
        public async Task HouseInfoAsync(string houseRole)
        {
            var theRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == houseRole);
            if(theRole == null)
            {
                await ReplyAsync("This house does not exist! Make sure you are spelling it correctly (case sensitive).");
                return;
            }
            House theHouse = await houseAccess.GetHouse(theRole.Id.ToString());
            User houseLeader = await userAccess.GetUserById(theHouse.Owner_FK);
            var msg = $"House {theHouse.Name} is currently under the service of liege {houseLeader.Name}.";
            await ReplyAsync(msg);
        }

        [Command("add_member")]
        [RequireRoleAttribute("House Leaders")]
        public async Task ApproveMemberAsync(IGuildUser user, string tier)
        {
            var tierRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == tier);
            var houseLeadersRole = Context.Guild.Roles.First(r => r.Name == "House Leaders");
            var guildUser = (SocketGuildUser)Context.User;
            var guildRole = guildUser.Roles.First(r => r.Name != "House Leaders" && r.Name != "Moderators" && r.Name != "Members" && r.Name != "@everyone" 
                && r.Name != "T1" && r.Name != "T2" && r.Name != "T3" && r.Name != "T4");
            House theHouse = await houseAccess.GetHouse(guildRole.Id.ToString());
            if(theHouse == null)
            {
                throw new Exception("You are not in a valid House. Please have an admin or moderator create one for you.");
            }
            User theUser = await userAccess.GetUser(user.Id.ToString());
            if(theUser == null)
            {
                var userName = user.Nickname == "" || user.Nickname == null ? user.Username : user.Nickname;
                theUser = await userAccess.Create(user.Id.ToString(), userName);
            }
            if(theUser.House_FK != null)
            {
                // User already in a house needs to leave first
                await ReplyAsync($"{user.Mention} is already in a House. They must use the command `!gcb leave_house` before being assigned to another.");
                return;
            }
            var userToAssign = (SocketGuildUser)Context.User;
            switch(tier) {
                case "T1":
                    await ReplyAsync("Cannot assign T1 member.");
                    return;
                case "T2":
                    await userAccess.AssignToHouse(theHouse.House_ID, 2, theUser.Player_ID);
                    await user.AddRoleAsync(guildRole);
                    await user.AddRoleAsync(tierRole);
                    await user.AddRoleAsync(houseLeadersRole);
                    await ReplyAsync($"{theUser.Name} has been added as a T2 to House {theHouse.Name}");
                    return;
                case "T3":
                    await userAccess.AssignToHouse(theHouse.House_ID, 3, theUser.Player_ID);
                    await user.AddRoleAsync(guildRole);
                    await user.AddRoleAsync(tierRole);
                    await ReplyAsync($"{theUser.Name} has been added as a T3 to House {theHouse.Name}");
                    return;
                case "T4":
                    await userAccess.AssignToHouse(theHouse.House_ID, 4, theUser.Player_ID);
                    await user.AddRoleAsync(guildRole);
                    await user.AddRoleAsync(tierRole);
                    await ReplyAsync($"{theUser.Name} has been added as a T4 to House {theHouse.Name}");
                    return;
                default:
                    await ReplyAsync("Please use the proper Tier terminology: T2, T3, or T4");
                    return;
            }
        }

        [Command("remove_member")]
        [RequireRoleAttribute("House Leaders")]
        public async Task RemoveMemberAsync(IGuildUser user)
        {
            User theUser = await userAccess.GetUser(user.Id.ToString());
            var socketUser = (SocketGuildUser)user;
            var guildRole = socketUser.Roles.FirstOrDefault(r => r.Name != "House Leaders" && r.Name != "Moderators" && r.Name != "Members" && r.Name != "@everyone"
                && r.Name != "T1" && r.Name != "T2" && r.Name != "T3" && r.Name != "T4");
            if(guildRole == null)
            {
                await ReplyAsync($"{theUser.Name} does not belong to any house.");
                return;
            }
            var callingUser = (SocketGuildUser)Context.User;
            User callingUserModel = await userAccess.GetUser(callingUser.Id.ToString());
            var myGuildRole = callingUser.Roles.FirstOrDefault(r => r.Name != "House Leaders" && r.Name != "Moderators" && r.Name != "Members" && r.Name != "@everyone"
                && r.Name != "T1" && r.Name != "T2" && r.Name != "T3" && r.Name != "T4");
            if(myGuildRole == null)
            {
                await ReplyAsync("You do not belong to a house.");
                return;
            }

            House theHouse = await houseAccess.GetHouse(guildRole.Id.ToString());
            if (theUser.House_FK != callingUserModel.House_FK)
            {
                await ReplyAsync($"{theUser.Name} does not belong to your house.");
            } else if (theUser.Role_FK == 1) {
                await ReplyAsync("You can't remove the head of the House.");
            } else if (theUser.Role_FK == 2 && callingUserModel.Role_FK != 1) {
                await ReplyAsync("You can't remove another T2. Only the House Liege can do that.");
            } else
            {
                var rolesToRemove = Context.Guild.Roles.Where(r => r.Name == "T4" || r.Name == "T2" || r.Name == "T3" || r.Name == "House Leaders");
                await socketUser.RemoveRolesAsync(rolesToRemove);
                await socketUser.RemoveRoleAsync(guildRole);
                await userAccess.RemoveFromHouse(theUser.Player_ID);
                await ReplyAsync($"{user.Mention} has been removed from {theHouse.Name}.");
            }

        }
    }
}

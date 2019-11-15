using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GoTCBot.Access.Abstractions;
using GoTCBot.Models;
using GoTCBot.Preconditions;

namespace GoTCBot.Modules
{
    public class PoliticsModule : ModuleBase<SocketCommandContext>
    {
        public IPlayerAccess _playerAccess;

        public PoliticsModule(
            IPlayerAccess playerAccess
        )
        {
            _playerAccess = playerAccess;
        }

        //[Command("create_house")]
        //public async Task CreateHouseAsync()
        //{

        //}

        [Command("declare")]
        [RequireRoleAttribute("House Leaders")]
        [Alias("war")]
        public async Task DeclareWarAsync(IRole role)
        {
            var guildUser = (SocketGuildUser) Context.User;
            var guildName = guildUser.Roles.First(r => r.Name != "House Leaders" && r.Name != "Moderators" && r.Name != "Members" && r.Name != "@everyone"
                && r.Name != "T1" && r.Name != "T2" && r.Name != "T3" && r.Name != "T4");
            if (guildName.Name == role.Name)
            {
                await ReplyAsync("You can't declare war on your own House!");
            } else
            {
                var msg = $":crossed_swords: {Context.User.Mention} of the guild {guildName.Mention} has declared war on {role.Mention} :crossed_swords:";
                SocketTextChannel warChat = (SocketTextChannel)Context.Guild.Channels.First(c => c.Name == "war");
                await warChat.SendMessageAsync(msg);
            }
        }

        [Command("peace")]
        [RequireRoleAttribute("House Leaders")]
        [Alias("makeup")]
        public async Task DeclarePeaceAsync(IRole role)
        {
            var guildUser = (SocketGuildUser)Context.User;
            var guildName = guildUser.Roles.First(r => r.Name != "House Leaders" && r.Name != "Moderators" && r.Name != "Members" && r.Name != "@everyone"
                && r.Name != "T1" && r.Name != "T2" && r.Name != "T3" && r.Name != "T4");
            if (guildName.Name == role.Name)
            {
                await ReplyAsync("You are already at peace with your own House...");
            }
            else
            {
                var msg = $":dove: {Context.User.Mention} of the guild {guildName.Mention} has amended their relationship with {role.Mention} :dove:";
                SocketTextChannel warChat = (SocketTextChannel)Context.Guild.Channels.First(c => c.Name == "war");
                await warChat.SendMessageAsync(msg);
            }
        }
    }
}

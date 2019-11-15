using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace PBBGDiscordBot.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {

        public AdminModule (
            
        )
        {
            
        }

        [Command("rules")]
        [Alias("info")]
        public async Task ServerRulesAsync()
        {
            var msg = "Test Message!";
            await ReplyAsync(msg);
        }
    }
}



using System.Threading.Tasks;
using Discord;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.Attributes
{
#pragma warning disable CS1998
    public class OniiSanAttribute : CheckBaseAttribute
    {
        public async override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return ctx.Guild.Id.Equals(Bot.Instance().cfg.MasterId);
        }
    }
}

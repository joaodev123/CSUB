using System.Threading.Tasks;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.Commands
{
    
    public class HelpCommand : BaseCommandModule
    {
        [Command("help"),Description("Ajuda")]
        public async Task helpCommand(CommandContext ctx)
        {
            await ctx.RespondAsync(embed: EmbedBase.HelpEmbed(ctx.CommandsNext, ctx));
        } 
    }
}
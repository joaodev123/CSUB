using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Logic;
using Logic.Models;

namespace Discord.Commands.Staff
{
    public class check : BaseCommandModule
    {
        [Command("check"), Description("Verifica o perfil de um membro"),Imouto,OniiSan]
        public async Task checkFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("check")]
        public async Task checkCommand(CommandContext ctx, [Description("Membro que será checado (Id/Menção)")] DiscordMember membro)
        {
            MembroModel perfil = new Membro().Find(x => x.DiscordId == membro.Id);
            if (perfil != null)
            {
                await ctx.RespondAsync(embed: await EmbedExtended.MemberProfile(perfil));
            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse membro não possui um perfil"));
            }
        }
    }
}
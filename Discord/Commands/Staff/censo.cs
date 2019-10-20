using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Logic.Models;

namespace Discord.Commands.Staff
{
    [Group("staff"),Description("Comandos da Staff"), Emoji(":tophat:"), Imouto, OniiSan]
    public class censo : BaseCommandModule
    {
        [GroupCommand()]
        public async Task groupCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
        [Command("censo"), Description("Mostra o censo de um membro.")]
        public async Task censoFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("censo")]
        public async Task censoShow(CommandContext ctx, [Description("O Membro (ID/Menção)")] DiscordMember membro)
        {
            CensoModel dados = new Logic.Censo().Find(x => x.DiscordId == membro.Id);
            if(dados == null)
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("O Membro não possui um censo nos registros."));
            }
            else
            {
                await ctx.RespondAsync(embed: await EmbedExtended.Censo(dados));
            }
        }
    }
}
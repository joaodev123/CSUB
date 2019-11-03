
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Logic;
using static Discord.Systems.Prisao;
using Logic.Models;

namespace Discord.Commands.Staff
{
    public class _prisao : BaseCommandModule
    {
        [Command("soltar"), Description("Solta um membro que foi preso manualmente."), Imouto]
        public async Task soltarFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("soltar")]
        public async Task soltar(CommandContext ctx, [Description("O membro que será solto")] DiscordMember member)
        {
            var infras = new Infracao().FindAll(x => x.IdInfrator == member.Id);
            if (infras.Count != 0)
            {
                var prisoes = new List<PrisaoModel>();
                infras.ForEach(x => prisoes.Add(new Prisao().Find(y => y.InfraId == x.Id)));
                var active = prisoes.Find(x => x.Elapsed == false);
                if (active == null)
                {
                    await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse membro não está preso."));
                }
                else
                {
                    await Soltar(active);
                    active.Elapsed = true;
                    new Prisao().Update(x => x.Id == active.Id, active);
                    await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Membro solto da prisão com sucesso!"));
                }
            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse membro possui uma ficha limpa."));
            }
        }
    }
}
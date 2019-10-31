using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Logic;
using Logic.Models;

namespace Discord.Commands.Staff
{
    [Group("infra"), Emoji(":closed_lock_with_key:"), Description("Sistema de Infracão"), Imouto, OniiSan]
    public class Infra : BaseCommandModule
    {
        [GroupCommand()]
        public async Task GroupCommand(CommandContext ctx)
        {
            await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command, ctx));
        }
        #region Add
        [Command("add"), Description("Adiciona uma infracao")]
        public async Task AddFail(CommandContext ctx)
        {
            await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        }
        [Command("add")]
        public async Task Add(CommandContext ctx,
        [Description("Membro que será punido (Menção / ID)")] DiscordMember membro)
        {
            InfracaoModel infra = new InfracaoModel
            {
                IdInfrator = membro.Id,
                IdStaff = ctx.Member.Id,
            };
            var info = await ctx.RespondAsync(embed: EmbedExtended.AsyncInfracaoEmbed(infra));
            var botMsg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Motivo"));
            var result = await ctx.Message.GetNextMessageAsync();
            infra.MotivoInfracao = result.Result.Content;
            await info.ModifyAsync(embed: EmbedExtended.AsyncInfracaoEmbed(infra));
            await botMsg.ModifyAsync(embed: EmbedBase.InputEmbed("Será Preso? [s/n]"));
            result = await result.Result.GetNextMessageAsync();
            infra.Preso = result.Result.Content.ToLowerInvariant()[0] == 'y' || result.Result.Content.ToLowerInvariant()[0] == 's';
            await info.ModifyAsync(embed: EmbedExtended.AsyncInfracaoEmbed(infra));
            if (infra.Preso)
            {
                await botMsg.ModifyAsync(embed: EmbedBase.InputEmbed("Tempo [xM, xH]"));
                result = await result.Result.GetNextMessageAsync();
                infra.Dados = new DadosPrisao();
                infra.Dados.Tempo = result.Result.Content;
            }
            else
            {
                infra.Dados = new DadosPrisao();
                infra.Dados.Tempo = "0s";
            }
            List<ulong?> ids = new List<ulong?>();
            membro.Roles.ToList().ForEach(x => ids.Add(x.Id));
            infra.Dados.Cargos = ids;
            await info.ModifyAsync(embed: EmbedExtended.AsyncInfracaoEmbed(infra));
            await botMsg.DeleteAsync();
            new Infracao().Insert(infra);
            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Infração Adicionada com Sucesso."));
        }
        #endregion
        #region List
        [Command("list"), Description("Lista as infrações")]
        public async Task ListFail(CommandContext ctx)
        {
            await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        }
        [Command("list")]
        public async Task List(CommandContext ctx,
        [Description("Membro (Menção/ID)")] DiscordMember membro)
        {
            var infracoes = new Infracao().FindAll(x => x.IdInfrator == membro.Id);
            if (infracoes.Count == 0)
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"{membro.Mention} possui ficha limpa!"));
            }
            else
            {
                foreach (InfracaoModel infra in infracoes)
                {
                    await ctx.RespondAsync(embed: EmbedExtended.AsyncInfracaoEmbed(infra));
                }
            }
        }
        #endregion
        #region Remove
        [Command("remove"), Description("Remove uma infração")]
        public async Task RemoveFail(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("remove")]
        public async Task Remove(CommandContext ctx, [Description("Membro (ID/Menção)")] DiscordMember membro)
        {
            var infras = new Infracao().FindAll(x => x.IdInfrator == membro.Id);
            if (infras.Count == 0)
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"O Membro {membro.Mention} possui uma ficha limpa!"));
            }
            else
            {
                var briefing = await ctx.RespondAsync(embed: await EmbedExtended.MemberBriefing(infras));
                var message = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("# da infração que será apagada"));
                var input = await ctx.Message.GetNextMessageAsync();
                var num = int.Parse(input.Result.Content.Split(" ")[0]);
                if (infras.Any(x => x.Id == num))
                {
                    var infra = infras.Find(x => x.Id == num);
                    new Infracao().Delete(infra);
                    await briefing.ModifyAsync(embed: EmbedBase.OutputEmbed($"Infração Apagada com sucesso.\n Dados :"));
                    await message.ModifyAsync(embed: EmbedExtended.AsyncInfracaoEmbed(infra));
                }
                else
                {
                    await briefing.DeleteAsync();
                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed($"Numero inválido, Comando cancelado."));
                }
            }
        }
        #endregion
    }
}
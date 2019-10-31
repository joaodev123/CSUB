using System.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Logic;
using Logic.Models;
using DSharpPlus.Interactivity.Enums;

namespace Discord.Commands.Staff
{
    [Group("stime"), Description("Eventos - Time (Staff)"), Emoji(":speaking_head:"), Imouto]
    public class time : BaseCommandModule
    {
        [GroupCommand()]
        public async Task groupCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command, ctx));
        [Command("criar"), Description("Cria um time")]
        public async Task criarFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("criar")]
        public async Task criar(CommandContext ctx,
        [Description("O Líder do time (Id/Menção)")] DiscordMember lider,
        [Description("O Nome do time."), RemainingText] string nome)
        {
            List<EventoModel> eventos = new Evento().FindAll(_ => true);
            if (eventos.Count > 0)
            {
                List<Page> pages = new List<Page>();
                eventos.ForEach(async e => pages.Add(new Page($"", new DiscordEmbedBuilder(await EmbedExtended.AsyncEventoEmbed(e)))));
                PaginationEmojis emojis = new PaginationEmojis
                {
                    Left = DiscordEmoji.FromName(ctx.Client, ":arrow_left:"),
                    Stop = DiscordEmoji.FromName(ctx.Client, ":stop_button:"),
                    Right = DiscordEmoji.FromName(ctx.Client, ":arrow_right:"),
                    SkipLeft = null,
                    SkipRight = null
                };
                var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed($"Selecione o evento. Depois clique em {emojis.Stop.ToString()} para confirmar."));
                await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages.ToArray(), emojis, PaginationBehaviour.WrapAround, PaginationDeletion.Default, TimeSpan.FromMinutes(30));
                var lastMsg = (await ctx.Channel.GetMessagesAfterAsync(msg.Id)).ToList().FirstOrDefault(x => x.Author == msg.Author && msg.Embeds.Count > 0);
                var id = int.Parse(lastMsg.Embeds[0].Fields.ToList().Find(x => x.Name == "Id").Value);
                var evento = eventos.Find(x => x.Id == id);
                await lastMsg.DeleteAsync();
                TimeModel time = new TimeModel();
                time.EventoId = evento.Id;
                time.Jogadores = new List<ulong> { lider.Id };
                time.LiderId = lider.Id;
                time.Nome = nome;
                new Time().Insert(time);
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Time criado com sucesso!"));
            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não há nenhum evento ativo no momento. Considere criar um!"));
            }
        }
        [Command("apagar"), Description("Apaga um time")]
        public async Task apagar(CommandContext ctx)
        {
            List<EventoModel> eventos = new Evento().FindAll(_ => true);
            if (eventos.Count > 0)
            {
                List<Page> pages = new List<Page>();
                eventos.ForEach(async e => pages.Add(new Page($"", new DiscordEmbedBuilder(await EmbedExtended.AsyncEventoEmbed(e)))));
                PaginationEmojis emojis = new PaginationEmojis
                {
                    Left = DiscordEmoji.FromName(ctx.Client, ":arrow_left:"),
                    Stop = DiscordEmoji.FromName(ctx.Client, ":stop_button:"),
                    Right = DiscordEmoji.FromName(ctx.Client, ":arrow_right:"),
                    SkipLeft = null,
                    SkipRight = null
                };
                var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed($"Selecione o evento. Depois clique em {emojis.Stop.ToString()} para confirmar."));
                await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages.ToArray(), emojis, PaginationBehaviour.WrapAround, PaginationDeletion.Default, TimeSpan.FromMinutes(30));
                var lastMsg = (await ctx.Channel.GetMessagesAfterAsync(msg.Id)).ToList().FirstOrDefault(x => x.Author == msg.Author && msg.Embeds.Count > 0);
                var id = int.Parse(lastMsg.Embeds[0].Fields.ToList().Find(x => x.Name == "Id").Value);
                var evento = eventos.Find(x => x.Id == id);
                await lastMsg.DeleteAsync();
                if (evento.Times == null)
                {
                    await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não há nenhum time neste evento."));
                }
                else
                {
                    List<TimeModel> times = new List<TimeModel>();
                    List<string> nomes = new List<string>();
                    Regex getUlong = new Regex(@"(?<!\.)\d+(?!\.)");
                    evento.Times.ForEach(x => times.Add(new Time().Find(y => y.Id == x)));
                    times.ForEach(x => nomes.Add($"{x.Id} - {x.Nome}"));
                    await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Time q será apagado [Número]"));
                    var list = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(nomes, $"Times em {evento.Nome}:"));
                    var responce = await ctx.Message.GetNextMessageAsync();
                    if (getUlong.IsMatch(responce.Result.Content))
                    {
                        int i = int.Parse(getUlong.Match(responce.Result.Content).ToString());
                        if (times.Any(x => x.Id == i))
                        {
                            new Time().Delete(x => x.Id == i);
                            await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Time apagado com sucesso."));
                        }
                        else
                        {
                            await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Número inválido. Comando Cancelado."));
                        }
                    }

                }
            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não há nenhum evento ativo no momento. Considere criar um!"));
            }
        }
    }
}

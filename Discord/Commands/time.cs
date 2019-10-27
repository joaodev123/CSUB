using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using Logic;
using Logic.Models;

namespace Discord.Commands
{
    [Group("time"), Emoji(":busts_in_silhouette:"), Description("Evento - Times")]
    public class time : BaseCommandModule
    {
        [GroupCommand()]
        public async Task groupCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command, ctx));
        [Command("criar"), Description("Cria um time para um evento")]
        public async Task createFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("criar")]
        public async Task create(CommandContext ctx, [Description("Nome do time"), RemainingText] String nome)
        {
            List<EventoModel> eventos = new Evento().FindAll(_ => true);
            if (eventos.Count > 0)
            {
                TimeModel time = new TimeModel
                {
                    Nome = nome,
                    LiderId = ctx.Member.Id,
                    Jogadores = new[] { ctx.Member.Id }.ToList()
                };
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
                var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed($"Selecione o evento que deseja participar. Depois clique em {emojis.Stop.ToString()} para confirmar."));
                await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages.ToArray(), emojis, PaginationBehaviour.WrapAround, PaginationDeletion.Default, TimeSpan.FromMinutes(30));
                var lastMsg = (await ctx.Channel.GetMessagesAfterAsync(msg.Id)).ToList().FirstOrDefault(x => x.Author == msg.Author && msg.Embeds.Count > 0);
                var id = int.Parse(lastMsg.Embeds[0].Fields.ToList().Find(x => x.Name == "Id").Value);
                var evento = eventos.Find(x => x.Id == id);
                if (new Time().Find(x => x.EventoId == id && x.LiderId == ctx.Member.Id) != null)
                {
                    await lastMsg.DeleteAsync();
                    await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Você já possui um time nesse evento."));
                }
                else
                {
                    time.EventoId = id;
                    await lastMsg.DeleteAsync();
                    if (evento.LimiteTimes == 0 || evento.Times == null || evento.Times.Count < evento.LimiteTimes)
                    {
                        new Time().Insert(time);
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Selecionado : {evento.Nome}. Time criado."));
                    }
                    else
                    {
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Inscrições para esse evento foram fechadas. Peça uma vaga aos organizadores do evento."));
                    }

                }

            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não há eventos ativos no momento. Inscrição de times foi desativada."));
            }
        }
        [Command("sair"), Description("Sai de um time. **Atenção Líderes : **Ao executar esse comando você ***apaga*** o time. Se for sair transfira a liderança primeiro.")]
        public async Task sair(CommandContext ctx)
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
                var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed($"Selecione o evento que deseja participar. Depois clique em {emojis.Stop.ToString()} para confirmar."));
                await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages.ToArray(), emojis, PaginationBehaviour.WrapAround, PaginationDeletion.Default, TimeSpan.FromMinutes(30));
                var lastMsg = (await ctx.Channel.GetMessagesAfterAsync(msg.Id)).ToList().FirstOrDefault(x => x.Author == msg.Author && msg.Embeds.Count > 0);
                var id = int.Parse(lastMsg.Embeds[0].Fields.ToList().Find(x => x.Name == "Id").Value);
                var evento = eventos.Find(x => x.Id == id);
                if (new Time().Find(x => x.EventoId == id && (x.Jogadores.Any(y => y == ctx.Member.Id) || x.Reservas.Any(y => y == ctx.Member.Id))) != null)
                {
                    var time = new Time().Find(x => x.EventoId == id && (x.Jogadores.Any(y => y == ctx.Member.Id) || x.Reservas.Any(y => y == ctx.Member.Id)));
                    await lastMsg.DeleteAsync();
                    if (time.LiderId != ctx.Member.Id)
                    {
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed($"Você deseja sair de {time.Nome}?.[s/n]"));
                        var input = await ctx.Message.GetNextMessageAsync();
                        switch (input.Result.Content.ToLowerInvariant().Trim()[0])
                        {
                            case 'y':
                            case 's':
                                if (time.Jogadores.Contains(ctx.Member.Id)) time.Jogadores.Remove(ctx.Member.Id);
                                if (time.Reservas.Contains(ctx.Member.Id)) time.Reservas.Remove(ctx.Member.Id);
                                new Time().Update(x => x.Id == time.Id, time);
                                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Você saiu do time com sucesso!"));
                                break;
                            default:
                                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Comando cancelado."));
                                break;
                        }
                    }
                    else
                    {
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed($"Você deseja **apagar** {time.Nome}?.[s/n]"));
                        var input = await ctx.Message.GetNextMessageAsync();
                        switch (input.Result.Content.ToLowerInvariant().Trim()[0])
                        {
                            case 'y':
                            case 's':
                                new Time().Delete(time);
                                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Time apagado com sucesso!"));
                                break;
                            default:
                                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Comando cancelado."));
                                break;
                        }
                    }

                }
                else
                {
                    await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Você não participa de nenhum time neste evento."));
                }

            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não há eventos ativos no momento. Inscrição de times foi desativada."));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    [Group("evento"), Emoji(":newspaper:"), Description("Eventos do discord")]
    public class evento : BaseCommandModule
    {
        [GroupCommand()]
        public async Task groupCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
        [Command("criar"), Description("Cria um evento"), Imouto]
        public async Task criarFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("criar")]
        public async Task criar(CommandContext ctx, [Description("Nome do evento"), RemainingText]string nome)
        {
            EventoModel evento = new EventoModel
            {
                Nome = nome
            };
            var dados = await ctx.RespondAsync(embed: await EmbedExtended.AsyncEventoEmbed(evento));
            var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Número max. de times [(s/n), número]"));
            var input = await ctx.Message.GetNextMessageAsync();
            if (char.IsNumber(input.Result.Content.ToLowerInvariant()[0]))
            {
                Regex getUlong = new Regex(@"([0-9])+");
                Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                int i = int.Parse(match.ToString());
                evento.LimiteTimes = i;
            }
            else
            {
                //TODO: Add an yes/no question to the Utils.
                switch (input.Result.Content.ToLowerInvariant()[0])
                {
                    case 's':
                    case 'y':
                        Regex getUlong = new Regex(@"([0-9])+");
                        if (getUlong.IsMatch(input.Result.Content.ToLowerInvariant()))
                        {
                            Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                            int i = int.Parse(match.ToString());
                            evento.LimiteTimes = i;
                        }
                        else
                        {
                            await msg.ModifyAsync(embed: EmbedBase.InputEmbed("O Número do limite."));
                            input = await input.Result.GetNextMessageAsync();
                            if (getUlong.IsMatch(input.Result.Content.ToLowerInvariant()))
                            {
                                Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                                int i = int.Parse(match.ToString());
                                evento.LimiteTimes = i;
                            }
                            else
                            {
                                await dados.DeleteAsync();
                                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Número inválido. Comando cancelado."));
                            }
                        }

                        break;
                    case 'n':
                        evento.LimiteTimes = 0;
                        break;
                }
            }
            await dados.ModifyAsync(embed: await EmbedExtended.AsyncEventoEmbed(evento));
            await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Número max. de jogadores [(s/n), número]"));
            input = await input.Result.GetNextMessageAsync();
            if (char.IsNumber(input.Result.Content.ToLowerInvariant()[0]))
            {
                Regex getUlong = new Regex(@"([0-9])+");
                Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                int i = int.Parse(match.ToString());
                evento.LimiteJogadores = i;
            }
            else
            {
                switch (input.Result.Content.ToLowerInvariant()[0])
                {
                    case 's':
                    case 'y':
                        Regex getUlong = new Regex(@"([0-9])+");
                        if (getUlong.IsMatch(input.Result.Content.ToLowerInvariant()))
                        {
                            Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                            int i = int.Parse(match.ToString());
                            evento.LimiteJogadores = i;
                        }
                        else
                        {
                            await msg.ModifyAsync(embed: EmbedBase.InputEmbed("O Número do limite."));
                            input = await input.Result.GetNextMessageAsync();
                            if (getUlong.IsMatch(input.Result.Content.ToLowerInvariant()))
                            {
                                Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                                int i = int.Parse(match.ToString());
                                evento.LimiteJogadores = i;
                            }
                            else
                            {
                                await dados.DeleteAsync();
                                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Número inválido. Comando cancelado."));
                            }
                        }

                        break;
                    case 'n':
                        evento.LimiteJogadores = 0;
                        break;
                }
            }
            await dados.ModifyAsync(embed: await EmbedExtended.AsyncEventoEmbed(evento));
            await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Número max. de reservas [(s/n), número]"));
            input = await input.Result.GetNextMessageAsync();
            if (char.IsNumber(input.Result.Content.ToLowerInvariant()[0]))
            {
                Regex getUlong = new Regex(@"([0-9])+");
                Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                int i = int.Parse(match.ToString());
                evento.LimiteReservas = i;
            }
            else
            {
                switch (input.Result.Content.ToLowerInvariant()[0])
                {
                    case 's':
                    case 'y':
                        Regex getUlong = new Regex(@"([0-9])+");
                        if (getUlong.IsMatch(input.Result.Content.ToLowerInvariant()))
                        {
                            Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                            int i = int.Parse(match.ToString());
                            evento.LimiteReservas = i;
                        }
                        else
                        {
                            await msg.ModifyAsync(embed: EmbedBase.InputEmbed("O Número do limite."));
                            input = await input.Result.GetNextMessageAsync();
                            if (getUlong.IsMatch(input.Result.Content.ToLowerInvariant()))
                            {
                                Match match = getUlong.Match(input.Result.Content.ToLowerInvariant());
                                int i = int.Parse(match.ToString());
                                evento.LimiteReservas = i;
                            }
                            else
                            {
                                await dados.DeleteAsync();
                                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Número inválido. Comando cancelado."));
                            }
                        }

                        break;
                    case 'n':
                        evento.LimiteReservas = 0;
                        break;
                }
            }
            await dados.ModifyAsync(embed: await EmbedExtended.AsyncEventoEmbed(evento));
            new Evento().Insert(evento);
            await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Evento adicionado com sucesso!"));
        }
        [Command("listar"), Description("Mostra todos os eventos disponíveis")]
        public async Task listar(CommandContext ctx)
        {
            List<EventoModel> eventos = new Evento().FindAll(_ => true);
            List<Page> pages = new List<Page>();
            eventos.ForEach(async e => pages.Add(new Page($"Lista de eventos ativos", new DiscordEmbedBuilder(await EmbedExtended.AsyncEventoEmbed(e)))));
            PaginationEmojis emojis = new PaginationEmojis
            {
                Left = DiscordEmoji.FromName(ctx.Client, ":arrow_left:"),
                Stop = DiscordEmoji.FromName(ctx.Client, ":stop_button:"),
                Right = DiscordEmoji.FromName(ctx.Client, ":arrow_right:"),
                SkipLeft = null,
                SkipRight = null
            };
            await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages.ToArray(), emojis, PaginationBehaviour.WrapAround, PaginationDeletion.Default, TimeSpan.FromMinutes(30));
        }

    }
}



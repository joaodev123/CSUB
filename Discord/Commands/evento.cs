using System;
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
using DSharpPlus.Interactivity.Enums;
using Logic;
using Logic.Models;

namespace Discord.Commands
{
    [Group("evento"), Emoji(":newspaper:"), Description("Eventos do discord")]
    public class evento : BaseCommandModule
    {
        [GroupCommand()]
        public async Task groupCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command, ctx));
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
            if (eventos.Count > 0)
            {
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
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não existem eventos ativos no momento."));
            }
        }
        [Command("apagar"), Description("Apaga um evento"), Imouto]
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
                var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed($"Selecione o evento a ser apagado, Depois clique em {emojis.Stop.ToString()} para confirmar."));
                await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages.ToArray(), emojis, PaginationBehaviour.WrapAround, PaginationDeletion.Default, TimeSpan.FromMinutes(30));
                var lastMsg = (await ctx.Channel.GetMessagesAfterAsync(msg.Id)).ToList().FirstOrDefault(x => x.Author == msg.Author && msg.Embeds.Count > 0);
                var id = int.Parse(lastMsg.Embeds[0].Fields.ToList().Find(x => x.Name == "Id").Value);
                var evento = eventos.Find(x => x.Id == id);
                await lastMsg.DeleteAsync();
                await msg.ModifyAsync(embed: EmbedBase.InputEmbed($"Selecionado : {evento.Nome}, Deseja apagar? [s/n]"));
                var input = await ctx.Message.GetNextMessageAsync();
                switch (input.Result.Content.ToLowerInvariant()[0])
                {
                    case 's':
                    case 'y':
                        new Evento().Delete(evento);
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Evento apagado com sucesso!"));
                        break;
                    case 'n':
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Comando cancelado."));
                        break;
                }
            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não há eventos cadastrados no sistema. Considere criar alguns!"));
            }
        }
        [Command("edit"), Description("Edita um evento"), Imouto]
        public async Task edit(CommandContext ctx)
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
                var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed($"Selecione o evento a ser apagado, Depois clique em {emojis.Stop.ToString()} para confirmar."));
                await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages.ToArray(), emojis, PaginationBehaviour.WrapAround, PaginationDeletion.Default, TimeSpan.FromMinutes(30));
                var lastMsg = (await ctx.Channel.GetMessagesAfterAsync(msg.Id)).ToList().FirstOrDefault(x => x.Author == msg.Author && msg.Embeds.Count > 0);
                var id = int.Parse(lastMsg.Embeds[0].Fields.ToList().Find(x => x.Name == "Id").Value);
                var evento = eventos.Find(x => x.Id == id);
                List<string> options = new List<string> { "Nome", "Limite de Jogadores/time", "Limite de reservas/time", "Quantidade max. de times" };
                await msg.ModifyAsync(embed: EmbedBase.InputEmbed($"Selecionado : {evento.Nome}"));
                await lastMsg.ModifyAsync(embed: EmbedBase.OrderedListEmbed(options, " Selecione o que deseja editar [Número]"));
                var input = await ctx.Message.GetNextMessageAsync();
                switch (input.Result.Content.ToLowerInvariant()[0])
                {
                    case '0':
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("O novo nome:"));
                        input = await input.Result.GetNextMessageAsync();
                        evento.Nome = input.Result.Content;
                        new Evento().Update(x => x.Id == evento.Id, evento);
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Nome modificado com sucesso!"));
                        break;
                    case '1':
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("O novo limite (0 para remover):"));
                        input = await input.Result.GetNextMessageAsync();
                        evento.LimiteJogadores = int.Parse(input.Result.Content);
                        new Evento().Update(x => x.Id == evento.Id, evento);
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Limite de jogadores modificado com sucesso!"));
                        break;
                    case '2':
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("O novo limite (0 para remover):"));
                        input = await input.Result.GetNextMessageAsync();
                        evento.LimiteReservas = int.Parse(input.Result.Content);
                        new Evento().Update(x => x.Id == evento.Id, evento);
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Limite de reservas modificado com sucesso!"));
                        break;
                    case '3':
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("O novo limite (0 para remover):"));
                        input = await input.Result.GetNextMessageAsync();
                        evento.LimiteTimes = int.Parse(input.Result.Content);
                        new Evento().Update(x => x.Id == evento.Id, evento);
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Limite de times modificado com sucesso!"));
                        break;
                    default:
                        await lastMsg.DeleteAsync();
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Comando cancelado. Valor inválido."));
                        break;
                }
                await lastMsg.ModifyAsync(embed: await EmbedExtended.AsyncEventoEmbed(evento));
            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Não há eventos cadastrados no sistema. Considere criar alguns!"));
            }
        }
    }
}



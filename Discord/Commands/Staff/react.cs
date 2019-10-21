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

namespace Discord.Commands.Staff
{
    [Group("react"), Imouto, Emoji(":book:"), Description("Esse grupo controla o sistema de ReactRole")]
    public class react : BaseCommandModule
    {
        [GroupCommand()]
        public async Task GroupCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
        [Command("channel"), Description("Cria/Remove Canais do Sistema de ReactRole")]
        public async Task channelFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("channel")]
        public async Task channel(CommandContext ctx, [Description("Operação (add/del)")]string action, [Description("O Canal (ID/Menção)")]DiscordChannel channel)
        {
            switch (action)
            {
                case "add":
                    var channels = new ReactChannel().Find(x => x.DiscordID == channel.Id);
                    if (channels != null) await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse canal já foi adicionado anteriormente."));
                    else
                    {
                        ReactChannelModel ch = new ReactChannelModel
                        {
                            DiscordID = channel.Id,
                        };
                        new ReactChannel().Insert(ch);
                        await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Canal adicionado com sucesso!"));
                    }
                    break;
                case "del":
                    channels = new ReactChannel().Find(x => x.DiscordID == channel.Id);
                    if (channels == null) await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse canal não foi adicionado."));
                    else
                    {
                        new ReactChannel().Delete(channels);
                        await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Canal removido com sucesso!"));
                    }
                    break;
            }
        }
        [Command("info"), Description("Mostra informações")]
        public async Task infoFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("info")]
        public async Task info(CommandContext ctx, [Description("O Canal (ID/Menção)")] DiscordChannel channel)
        {
            var info = new ReactChannel().Find(x => x.DiscordID == channel.Id);
            if (info != null)
            {
                await ctx.RespondAsync(embed: await EmbedExtended.ChannelInfo(info));
            }
            else
            {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse canal não foi adicionado."));
            }
        }
        [Command("categoria"), Description("Adiciona/Remove/Edita Categorias")]
        public async Task categoriaFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("categoria")]
        public async Task categoria(CommandContext ctx, [Description("Operação (add/del/edit)")] string operation)
        {
            switch (operation)
            {
                case "add":
                    var message = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Canal do discord (ID/Menção)"));
                    var responce = await ctx.Message.GetNextMessageAsync();
                    Regex getUlong = new Regex(@"(?<!\.)\d+(?!\.)");
                    var match = getUlong.Match(responce.Result.Content).ToString();
                    if (!String.IsNullOrWhiteSpace(match))
                    {
                        ulong id = ulong.Parse(match);
                        if (new ReactChannel().Find(x => x.DiscordID == id) != null)
                        {
                            DiscordChannel ch = ctx.Guild.GetChannel(id);

                            await message.ModifyAsync(embed: EmbedBase.InputEmbed("Nome da Categoria"));
                            responce = await responce.Result.GetNextMessageAsync();
                            string nome = responce.Result.Content;
                            string mensagem = "";
                            await message.ModifyAsync(embed: EmbedBase.InputEmbed("Gostaria de adicionar uma mensagem na categoria? [s/n]"));
                            responce = await responce.Result.GetNextMessageAsync();
                            if (responce.Result.Content[0] == 's')
                            {
                                await message.ModifyAsync(embed: EmbedBase.InputEmbed("Mensagem (Sobre :)"));
                                responce = await responce.Result.GetNextMessageAsync();
                                mensagem = responce.Result.Content;
                            }
                            ReactCategoryModel cat = new ReactCategoryModel
                            {
                                ChannelId = ch.Id,
                                Name = nome,
                                Description = mensagem,
                            };
                            var msg = await ch.SendMessageAsync(embed: await EmbedExtended.ReactCategory(cat));
                            cat.MessageId = msg.Id;
                            new ReactCategory().Insert(cat);
                            await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Categoria adicionada com sucesso!"));
                        }
                        else
                        {
                            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse canal não foi configurado."));
                        }

                    }
                    else
                    {
                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Mensagem inválida. Comando Cancelado"));
                    }
                    break;
                case "del":
                    message = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Canal do discord (ID/Menção)"));
                    responce = await ctx.Message.GetNextMessageAsync();
                    getUlong = new Regex(@"(?<!\.)\d+(?!\.)");
                    match = getUlong.Match(responce.Result.Content).ToString();
                    if (!String.IsNullOrWhiteSpace(match))
                    {
                        ulong id = ulong.Parse(match);
                        if (new ReactChannel().Find(x => x.DiscordID == id) != null)
                        {
                            DiscordChannel ch = ctx.Guild.GetChannel(id);
                            var cat = new ReactCategory().FindAll(_ => true);
                            List<string> names = new List<string>();
                            cat.ForEach(x => names.Add(x.Name));
                            await message.ModifyAsync(embed: EmbedBase.InputEmbed("Numero da categoria que você quer apagar"));
                            var list = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(names, "Categorias"));
                            responce = await responce.Result.GetNextMessageAsync();
                            match = getUlong.Match(responce.Result.Content).ToString();
                            if (!String.IsNullOrWhiteSpace(match))
                            {
                                var i = int.Parse(match);
                                if (i <= names.Count)
                                {
                                    var category = cat[i];
                                    var catmsg = await ch.GetMessageAsync(category.MessageId);
                                    await message.ModifyAsync(embed: EmbedBase.InputEmbed($"Gostaria de apagar a categoria {category.Name}? [s/n]"));
                                    await list.ModifyAsync(embed: await EmbedExtended.ReactCategory(category));
                                    responce = await ctx.Message.GetNextMessageAsync();
                                    if (responce.Result.Content[0] == 's')
                                    {
                                        new ReactCategory().Delete(category);
                                        await list.DeleteAsync();
                                        await catmsg.DeleteAsync();
                                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed($"Categoria {category.Name} apagada com sucesso!"));
                                    }
                                }
                                else
                                {
                                    await list.DeleteAsync();
                                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Número maior que o maximo da lista. Comando Cancelado."));
                                }
                            }
                        }
                        else
                        {
                            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse canal não foi configurado."));
                        }
                    }
                    else
                    {
                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Mensagem inválida. Comando Cancelado"));
                    }
                    break;
                case "edit":
                    message = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Canal do discord (ID/Menção)"));
                    responce = await ctx.Message.GetNextMessageAsync();
                    getUlong = new Regex(@"(?<!\.)\d+(?!\.)");
                    match = getUlong.Match(responce.Result.Content).ToString();
                    if (!String.IsNullOrWhiteSpace(match))
                    {
                        ulong id = ulong.Parse(match);
                        if (new ReactChannel().Find(x => x.DiscordID == id) != null)
                        {
                            DiscordChannel ch = ctx.Guild.GetChannel(id);
                            var cat = new ReactCategory().FindAll(_ => true);
                            List<string> names = new List<string>();
                            cat.ForEach(x => names.Add(x.Name));
                            await message.ModifyAsync(embed: EmbedBase.InputEmbed("Numero da categoria que você quer editar"));
                            var list = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(names, "Categorias"));
                            responce = await responce.Result.GetNextMessageAsync();
                            match = getUlong.Match(responce.Result.Content).ToString();
                            if (!String.IsNullOrWhiteSpace(match))
                            {
                                var i = int.Parse(match);
                                if (i <= names.Count)
                                {
                                    var category = cat[i];
                                    List<string> options = new List<string> { "nome", "mensagem" };
                                    await message.ModifyAsync(embed: EmbedBase.InputEmbed("Numero da opção a ser editada"));
                                    await list.ModifyAsync(embed: EmbedBase.OrderedListEmbed(options, "Opções"));
                                    responce = await responce.Result.GetNextMessageAsync();
                                    match = getUlong.Match(responce.Result.Content).ToString();
                                    if (!String.IsNullOrWhiteSpace(match))
                                    {
                                        i = int.Parse(match);
                                        if (i <= options.Count)
                                        {
                                            switch (i)
                                            {
                                                case 0:
                                                    await list.DeleteAsync();
                                                    await message.ModifyAsync(embed: EmbedBase.InputEmbed("O novo nome da categoria"));
                                                    responce = await responce.Result.GetNextMessageAsync();
                                                    category.Name = responce.Result.Content;
                                                    var catmsg = await ch.GetMessageAsync(category.MessageId);
                                                    await catmsg.ModifyAsync(embed: await EmbedExtended.ReactCategory(category));
                                                    new ReactCategory().Update(x => x.Id == category.Id, category);
                                                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Categoria modificada com sucesso!"));
                                                    break;
                                                case 1:
                                                    await list.DeleteAsync();
                                                    await message.ModifyAsync(embed: EmbedBase.InputEmbed("A nova mensagem da categoria"));
                                                    responce = await responce.Result.GetNextMessageAsync();
                                                    category.Description = responce.Result.Content;
                                                    catmsg = await ch.GetMessageAsync(category.MessageId);
                                                    await catmsg.ModifyAsync(embed: await EmbedExtended.ReactCategory(category));
                                                    new ReactCategory().Update(x => x.Id == category.Id, category);
                                                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Categoria modificada com sucesso!"));
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            await list.DeleteAsync();
                                            await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Número maior que o maximo da lista. Comando Cancelado."));
                                        }
                                    }
                                    else
                                    {
                                        await list.DeleteAsync();
                                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Mensagem inválida. Comando Cancelado"));
                                    }
                                }
                                else
                                {
                                    await list.DeleteAsync();
                                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Número maior que o maximo da lista. Comando Cancelado."));
                                }
                            }
                        }
                        else
                        {
                            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Esse canal não foi configurado."));
                        }
                    }
                    else
                    {
                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Mensagem inválida. Comando Cancelado"));
                    }
                    break;
                default:
                    break;

            }
        }
        [Command("role"), Description("Adiciona/Remove Cargos")]
        public async Task roleFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("role")]
        public async Task role(CommandContext ctx, [Description("Operação (add/del)")] string operation)
        {
            switch (operation)
            {
                case "add":
                    var message = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Canal do discord (ID/Menção)"));
                    var responce = await ctx.Message.GetNextMessageAsync();
                    Regex getUlong = new Regex(@"(?<!\.)\d+(?!\.)");
                    var match = getUlong.Match(responce.Result.Content).ToString();
                    if (!String.IsNullOrWhiteSpace(match))
                    {
                        ulong id = ulong.Parse(match);
                        if (new ReactChannel().Find(x => x.DiscordID == id) != null)
                        {
                            DiscordChannel ch = ctx.Guild.GetChannel(id);
                            var cat = new ReactCategory().FindAll(_ => true);
                            List<string> names = new List<string>();
                            cat.ForEach(x => names.Add(x.Name));
                            await message.ModifyAsync(embed: EmbedBase.InputEmbed("Numero da categoria que você quer apagar"));
                            var list = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(names, "Categorias"));
                            responce = await responce.Result.GetNextMessageAsync();
                            match = getUlong.Match(responce.Result.Content).ToString();
                            if (!String.IsNullOrWhiteSpace(match))
                            {
                                var i = int.Parse(match);
                                if (i <= names.Count)
                                {
                                    var category = cat[i];
                                    await list.DeleteAsync();
                                    await message.ModifyAsync(embed: EmbedBase.InputEmbed("Emoji (Id/Menção)"));
                                    responce = await responce.Result.GetNextMessageAsync();
                                    var getEmoji = new Regex(@"(\:\w*\:)");
                                    match = getEmoji.Match(responce.Result.Content).ToString();
                                    if (!String.IsNullOrWhiteSpace(match))
                                    {
                                        var emoji = DiscordEmoji.FromName(ctx.Client, match);
                                        await message.ModifyAsync(embed: EmbedBase.InputEmbed("Cargo (Id/Menção)"));
                                        responce = await responce.Result.GetNextMessageAsync();
                                        match = getUlong.Match(responce.Result.Content).ToString();
                                        if (!String.IsNullOrWhiteSpace(match))
                                        {
                                            id = ulong.Parse(match);
                                            var role = ctx.Guild.GetRole(id);
                                            ReactRoleModel roleModel = new ReactRoleModel
                                            {
                                                RoleId = role.Id,
                                                EmojiId = emoji.Id,
                                                CategoryId = category.Id
                                            };
                                            new ReactRole().Insert(roleModel);
                                            var catmsg = await ch.GetMessageAsync(category.MessageId);
                                            await catmsg.CreateReactionAsync(emoji);
                                            category = new ReactCategory().Find(x => x.Id == category.Id);
                                            await catmsg.ModifyAsync(embed: await EmbedExtended.ReactCategory(category));
                                            await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Cargo adicionado com sucesso!"));
                                        }
                                        else
                                        {
                                            await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Mensagem inválida. Comando Cancelado"));
                                        }

                                    }
                                    else
                                    {
                                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Mensagem inválida. Comando Cancelado"));
                                    }

                                }
                            }
                            else
                            {
                                await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Número inválido. Comando Cancelado"));
                            }
                        }
                    }
                    else
                    {
                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Mensagem inválida. Comando Cancelado"));
                    }
                    break;
                case "del":
                    message = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Canal do discord (ID/Menção)"));
                    responce = await ctx.Message.GetNextMessageAsync();
                    getUlong = new Regex(@"(?<!\.)\d+(?!\.)");
                    match = getUlong.Match(responce.Result.Content).ToString();
                    if (!String.IsNullOrWhiteSpace(match))
                    {
                        ulong id = ulong.Parse(match);
                        if (new ReactChannel().Find(x => x.DiscordID == id) != null)
                        {
                            DiscordChannel ch = ctx.Guild.GetChannel(id);
                            var cat = new ReactCategory().FindAll(_ => true);
                            List<string> names = new List<string>();
                            cat.ForEach(x => names.Add(x.Name));
                            await message.ModifyAsync(embed: EmbedBase.InputEmbed("Numero da categoria que você quer apagar"));
                            var list = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(names, "Categorias"));
                            responce = await responce.Result.GetNextMessageAsync();
                            match = getUlong.Match(responce.Result.Content).ToString();
                            if (!String.IsNullOrWhiteSpace(match))
                            {
                                var i = int.Parse(match);
                                if (i <= names.Count)
                                {
                                    var category = cat[i];
                                    List<ReactRoleModel> roles = new List<ReactRoleModel>();
                                    if (category.Roles != null)
                                    {
                                        category.Roles.ForEach(x => roles.Add(new ReactRole().Find(y => y.Id == x)));
                                    }
                                    if (roles.Count > 0)
                                    {
                                        List<string> role_string = new List<string>();
                                        foreach (var r in roles)
                                        {
                                            var role = ctx.Guild.GetRole(r.RoleId);
                                            var emoji = DiscordEmoji.FromGuildEmote(ctx.Client, r.EmojiId);
                                            role_string.Add($"{emoji} - {role.Name}");
                                        }
                                        await list.ModifyAsync(embed: EmbedBase.OrderedListEmbed(role_string, "Cargos"));
                                        await message.ModifyAsync(embed: EmbedBase.InputEmbed("Numero do cargo à ser deletado"));
                                        responce = await responce.Result.GetNextMessageAsync();
                                        match = getUlong.Match(responce.Result.Content).ToString();
                                        if (!String.IsNullOrWhiteSpace(match))
                                        {
                                            i = int.Parse(match);
                                            if (roles.Count >= i)
                                            {
                                                var rr = roles[i];
                                                await list.DeleteAsync();
                                                var emoji = DiscordEmoji.FromGuildEmote(ctx.Client, rr.EmojiId);
                                                new ReactRole().Delete(rr);
                                                category = new ReactCategory().Find(x => x.Id == category.Id);
                                                var catmsg = await ch.GetMessageAsync(category.MessageId);
                                                var members = await catmsg.GetReactionsAsync(emoji);
                                                members.ToList().ForEach(async x => await catmsg.DeleteReactionAsync(emoji, x));
                                                await catmsg.ModifyAsync(embed: await EmbedExtended.ReactCategory(category));
                                                await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Cargo apagado com sucesso!"));
                                            }

                                        }
                                    }
                                    else
                                    {
                                        await list.DeleteAsync();
                                        await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Essa categoria não possui cargos."));
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;
using Logic;
using Logic.Models;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace Discord.Commands
{
    public class Censo : BaseCommandModule
    {
        [Command("censo"), Description("Cria o censo comunitário"), OniiSan]
        public async Task censoFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));

        [Command("censo")]
        public async Task censo(CommandContext ctx, [Description("editar/fazer")]string operacao = "fazer")
        {
            switch (operacao)
            {
                case "fazer":
                    MembroModel membro = new Membro().Find(x => x.DiscordId == ctx.Member.Id);
                    if (membro == null)
                    {
                        membro = new MembroModel
                        {
                            DiscordId = ctx.Member.Id
                        };
                        new Membro().Insert(membro);
                    }
                    if (membro.Censo != 0)
                    {
                        await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Um censo comunitário já existe.\n" +
                        $"Use `{ctx.Prefix}{ctx.Command.Name} editar` para editar o seu censo!"));
                    }
                    else
                    {
                        await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("O censo será feito **no privado**. Uma mensagem com mais detalhes será enviada."));
                        var Dm = await ctx.Member.CreateDmChannelAsync();
                        CensoModel _Censo = new CensoModel
                        {
                            Timestamp = DateTime.Now,
                            DiscordId = ctx.Member.Id
                        };
                        var msg = await Dm.SendMessageAsync(embed: EmbedBase.InputEmbed("Idade"));
                        var responce = await Dm.GetNextMessageAsync(x => x.Author == ctx.User, TimeSpan.FromMinutes(30));
                        int idade = 0;
                        responce.Result.Content.Split(" ").ToList().ForEach(x => int.TryParse(x, out idade));
                        if (idade != 0)
                        {
                            _Censo.Idade = idade;
                        }
                        int.TryParse(responce.Result.Content, out idade);
                        if (idade != 0)
                        {
                            _Censo.Idade = idade;
                        }
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Email"));
                        responce = await responce.Result.GetNextMessageAsync();
                        string email = "";
                        Regex check = new Regex(@"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*");
                        email = check.Match(responce.Result.Content).ToString();
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            _Censo.Email = email;
                        }
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Jogos"));
                        responce = await responce.Result.GetNextMessageAsync();
                        _Censo.Jogos = responce.Result.Content;
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Você é brasileiro(a) ? Responda com [sim/não]"));
                        responce = await responce.Result.GetNextMessageAsync();
                        if (responce.Result.Content.ToLowerInvariant()[0] == 's')
                        {
                            await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Sigla do seu estado"));
                            responce = await responce.Result.GetNextMessageAsync();
                            if (Enum.GetNames(typeof(Estado)).Contains(responce.Result.Content.ToUpperInvariant()))
                            {
                                _Censo.UF = (Estado)Enum.Parse(typeof(Estado), responce.Result.Content.ToUpperInvariant());
                            }
                            else
                            {
                                _Censo.UF = Estado.OTHER;
                            }
                        }
                        else
                        {
                            _Censo.UF = Estado.OTHER;
                        }
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Como chegou à UBGE"));
                        responce = await responce.Result.GetNextMessageAsync();
                        _Censo.Origem = responce.Result.Content;
                        await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Fala alguma outra língua alem do português?"));
                        responce = await responce.Result.GetNextMessageAsync();
                        _Censo.Idiomas = responce.Result.Content;
                        new Logic.Censo().Insert(_Censo);
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Censo Concluído."));

                    }
                    break;
                case "editar":
                    CensoModel censo = new Logic.Censo().Find(x => x.DiscordId == ctx.Member.Id);
                    if (censo == null)
                    {
                        await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Você não fez o censo ainda.\n" +
                        "Faça-o com {ctx.Prefix}`{ctx.Command.Name} fazer`"));
                    }
                    else
                    {
                        await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("O censo será editado no privado. Uma mensagem será enviada em breve!"));
                        var Dm = await ctx.Member.CreateDmChannelAsync();

                        List<string> options = new List<string> { "Email", "Idade", "Idiomas", "Jogos", "Como chegou à UBGE" };
                        var message = await Dm.SendMessageAsync(embed: EmbedBase.OrderedListEmbed(options, "Selecione uma das opções"));
                        var responce = await Dm.GetNextMessageAsync(x => x.Author.Id == ctx.Member.Id);
                        if (char.IsDigit(responce.Result.Content[0]))
                        {
                            var value = int.Parse(responce.Result.Content[0].ToString());
                            await message.ModifyAsync(embed: EmbedBase.InputEmbed($"Digite o(s) novo(s) {options[value]}:"));
                            responce = await responce.Result.GetNextMessageAsync();
                            await Modify(censo, value, message, responce);
                        }
                        else
                        {
                            var option = options.Find(x => x.ToLowerInvariant().Contains(responce.Result.Content.ToLowerInvariant()));
                            if (!String.IsNullOrWhiteSpace(option))
                            {
                                var value = options.IndexOf(option);
                                await message.ModifyAsync(embed: EmbedBase.InputEmbed($"Digite o(s) novo(s) {options[value]}:"));
                                responce = await responce.Result.GetNextMessageAsync();
                                await Modify(censo, value, message, responce);
                            }
                        }
                    }
                    break;
            }
        }
        public async Task Modify(CensoModel censo, int value, DiscordMessage message, InteractivityResult<DiscordMessage> responce)
        {
            switch (value)
            {
                case 0:
                    Regex check = new Regex(@"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*");
                    var email = check.Match(responce.Result.Content).ToString();
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        censo.Email = email;
                    }
                    new Logic.Censo().Update(x => x.Id == censo.Id, censo);
                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Email modificado com sucesso!"));
                    break;
                case 1:
                    int idade = 0;
                    responce.Result.Content.Split(" ").ToList().ForEach(x => int.TryParse(x, out idade));
                    if (idade != 0)
                    {
                        censo.Idade = idade;
                    }
                    int.TryParse(responce.Result.Content, out idade);
                    if (idade != 0)
                    {
                        censo.Idade = idade;
                    }
                    new Logic.Censo().Update(x => x.Id == censo.Id, censo);
                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Idade modificada com sucesso!"));
                    break;
                case 2:
                    censo.Idiomas = responce.Result.Content;
                    new Logic.Censo().Update(x => x.Id == censo.Id, censo);
                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Idiomas modificados com sucesso!"));
                    break;
                case 3:
                    censo.Jogos = responce.Result.Content;
                    new Logic.Censo().Update(x => x.Id == censo.Id, censo);
                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Jogos modificados com sucesso!"));
                    break;
                case 4:
                    censo.Origem = responce.Result.Content;
                    new Logic.Censo().Update(x => x.Id == censo.Id, censo);
                    await message.ModifyAsync(embed: EmbedBase.OutputEmbed("Origem modificada com sucesso!"));
                    break;
                default:
                    break;
            }
        }
    }
}
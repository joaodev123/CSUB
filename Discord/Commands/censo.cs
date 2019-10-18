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
namespace Discord.Commands
{
    public class Censo : BaseCommandModule
    {
        [Command("censo"), Description("Cria o censo comunitário"), OniiSan]
        public async Task censoFailed(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));

        [Command("censo")]
        public async Task censo(CommandContext ctx, [Description("editar/fazer")]string operacao = "fazer")
        {
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
                CensoModel censo = new CensoModel
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
                    censo.Idade = idade;
                }
                int.TryParse(responce.Result.Content, out idade);
                if (idade != 0)
                {
                    censo.Idade = idade;
                }
                await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Email"));
                responce = await responce.Result.GetNextMessageAsync();
                string email = "";
                Regex check = new Regex(@"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*");
                email = check.Match(responce.Result.Content).ToString();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    censo.Email = email;
                }
                await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Jogos"));
                responce = await responce.Result.GetNextMessageAsync();
                censo.Jogos = responce.Result.Content;
                await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Você é brasileiro(a) ? Responda com [sim/não]"));
                responce = await responce.Result.GetNextMessageAsync();
                if (responce.Result.Content[0] == 's')
                {
                    await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Sigla do seu estado"));
                    responce = await responce.Result.GetNextMessageAsync();
                    if (Enum.GetNames(typeof(Estado)).Contains(responce.Result.Content.ToUpperInvariant()))
                    {
                        censo.UF = (Estado)Enum.Parse(typeof(Estado), responce.Result.Content.ToUpperInvariant());
                    }
                    else
                    {
                        censo.UF = Estado.OTHER;
                    }
                }
                else
                {
                    censo.UF = Estado.OTHER;
                }
                await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Como chegou à UBGE"));
                responce = await responce.Result.GetNextMessageAsync();
                censo.Origem = responce.Result.Content;
                await msg.ModifyAsync(embed: EmbedBase.InputEmbed("Fala alguma outra língua alem do português?"));
                responce = await responce.Result.GetNextMessageAsync();
                censo.Idiomas = responce.Result.Content;
                new Logic.Censo().Insert(censo);
                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Censo Concluído."));

            }
        }
    }
}
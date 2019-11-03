using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Logic;
using Logic.Models;

namespace Discord.Utils
{
    public static class EmbedExtended
    {
        public static DiscordEmbed AsyncInfracaoEmbed(InfracaoModel infra)
        {

            string infraInfo = $"Informaçoes da Infração #{infra.Id}:\n" +
            $"Membro : {(infra.IdInfrator == 0 ? "Sem Dados" : $"<@{infra.IdInfrator}>")}\n" +
            $"Staff : {(infra.IdStaff == 0 ? "Sem Dados" : $"<@{infra.IdStaff}>")}\n" +
            $"Motivo : {(String.IsNullOrWhiteSpace(infra.MotivoInfracao) ? "Sem Dados" : $"{infra.MotivoInfracao}")}\n" +
            $"Preso : {(infra.Preso ? ":white_check_mark:" : $":negative_squared_cross_mark:")}\n";
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder
            .WithAuthor("Detalhes da Infração")
            .WithDescription(infraInfo)
            .WithColor(DiscordColor.Brown);
            if (infra.Preso)
            {

                PrisaoModel prisao = new Prisao().Find(x => x.InfraId == infra.Id);
                if (prisao != null)
                {
                    string infraCargos = "";
                    if (prisao.Cargos != null) prisao.Cargos.ForEach(x => infraCargos += $"<@&{x}>, ");
                    string infraDados = $"Cargos : {(String.IsNullOrWhiteSpace(infraCargos) ? "Sem Dados" : $"{infraCargos}")}\n" +
                    $"Tempo : {(prisao.Duracao.Equals(new TimeSpan()) ? "Sem Dados" : $"{prisao.Duracao.ToString(@"dd\.hh\:mm")}hrs")}";
                    builder.AddField("Dados", infraDados);
                }

            }
            return builder.Build();
        }
#pragma warning disable CS1998
        public static async Task<DiscordEmbed> AsyncEventoEmbed(EventoModel evento)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            string dados = "";
            List<TimeModel> times = new List<TimeModel>();
            if (evento.Times != null)
            {
                evento.Times.ForEach(x => times.Add(new Time().Find(y => y.Id == x)));
            }
            times.ForEach(x => dados += $"{x.Nome}, ");
            builder
            .AddField("Id", evento.Id == 0 ? "Desconhecido" : evento.Id.ToString())
            .AddField("Numero máximo de times", evento.LimiteTimes == 0 ? "Não há limite" : evento.LimiteTimes.ToString(), true)
            .AddField("Numero máximo de jogadores", evento.LimiteJogadores == 0 ? "Não há limite" : evento.LimiteJogadores.ToString(), true)
            .AddField("Numero máximo de reservas", evento.LimiteReservas == 0 ? "Não há limite" : evento.LimiteReservas.ToString(), true)
            .AddField("Times", $"{(String.IsNullOrEmpty(dados) ? "Nenhum Time foi registrado" : dados)}", true)
            .WithAuthor($"Eventos : {evento.Nome}")
            .WithColor(DiscordColor.Lilac);
            return builder.Build();
        }

        public static async Task<DiscordEmbed> Censo(CensoModel dados)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            DiscordUser user = await Bot.Instance().client.GetUserAsync(dados.DiscordId);
            builder
            .WithColor(DiscordColor.Goldenrod)

            .WithDescription($"Censo Feito em : {TimeZoneInfo.ConvertTimeFromUtc(dados.Timestamp, TimeZoneInfo.FindSystemTimeZoneById("Brazil/East")).ToString(new CultureInfo("pt-BR"))}")
            .AddField("Idiomas", dados.Idiomas, true)
            .AddField("Jogos", dados.Jogos, true)
            .AddField("Origem", dados.Origem, true)
            .AddField("UF", dados.UF.ToString(), true)
            .AddField("Email", dados.Email, true)
            .AddField("Idade", dados.Idade.ToString(), true)
            .WithAuthor($"Censo do(a) {user.Username}#{user.Discriminator}", null, user.AvatarUrl);
            return builder.Build();
        }

        public static async Task<DiscordEmbed> MemberProfile(MembroModel profile)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            DiscordUser user = await Bot.Instance().client.GetUserAsync(profile.DiscordId);
            List<InfracaoModel> infracoes = new List<InfracaoModel>();
            string InfraStr = "";
            if (profile.Infracoes != null)
            {
                profile.Infracoes.ForEach(id => infracoes.Add(new Infracao().Find(x => x.Id == id)));
            }
            builder
            .AddField("Numero de Infrações", infracoes.Count.ToString(), true)
            .AddField("Cargo", profile.Cargo.ToString(), true)
            .WithColor(DiscordColor.Blurple)
            .WithAuthor($"Perfil de {user.Username}#{user.Discriminator}", null, user.AvatarUrl);
            if (infracoes.Count > 0)
            {
                foreach (var infra in infracoes)
                {
                    InfraStr += $"`#{infra.Id}` - `{infra.MotivoInfracao} (`<@{infra.IdStaff}>`)`\n";
                }
            }
            else
            {
                InfraStr += "`Nenhuma Infração.`";
            }
            builder.AddField("Infrações", InfraStr);
            return builder.Build();
        }

        public async static Task<DiscordEmbed> ChannelInfo(ReactChannelModel info)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            DiscordChannel ch = await Bot.Instance().client.GetChannelAsync(info.DiscordID);
            List<ReactCategoryModel> categorias = new List<ReactCategoryModel>();
            if (info.Categories != null)
            {
                info.Categories.ForEach(x => categorias.Add(new ReactCategory().Find(y => y.Id == x)));
            }
            builder
            .AddField("Numero de Categorias", categorias.Count.ToString())
            .WithColor(DiscordColor.Blurple)
            .WithAuthor($"Informações de {ch.Name}");
            return builder.Build();
        }

        public static async Task<DiscordEmbed> MemberBriefing(List<InfracaoModel> infraList)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            DiscordUser user = await Bot.Instance().client.GetUserAsync(infraList[0].IdInfrator);
            string data = "";
            infraList.ForEach(x => data += $"`[#{x.Id}]` - `{x.MotivoInfracao} (`<@{x.IdStaff}>`)`\n");
            builder
            .WithDescription(data)
            .WithAuthor($"Infrações de {user.Username}#{user.Discriminator}", null, user.AvatarUrl);
            return builder.Build();
        }

        internal static async Task<DiscordEmbed> ReactCategory(ReactCategoryModel cat)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            List<ReactRoleModel> roles = new List<ReactRoleModel>();
            builder
            .WithColor(DiscordColor.CornflowerBlue)
            .WithDescription("Clique na reação para obter o cargo. Remova a reação para tirar o cargo.")
            .WithAuthor($"Categoria : {cat.Name}");
            if (!String.IsNullOrWhiteSpace(cat.Description))
            {
                builder.AddField("Info", cat.Description, true);
            }
            if (cat.Roles != null) cat.Roles.ForEach(x => roles.Add(new ReactRole().Find(y => y.Id == x)));
            string role_string = "";
            foreach (var role in roles)
            {
                DiscordChannel ch = await Bot.Instance().client.GetChannelAsync(cat.ChannelId);
                var e = await ch.Guild.GetEmojiAsync(role.EmojiId);
                var r = ch.Guild.GetRole(role.RoleId);
                role_string += $"{e.ToString()} - {r.Name}\n";
            }
            if (!String.IsNullOrWhiteSpace(role_string)) { builder.AddField("Cargos", role_string); }
            return builder.Build();
        }
    }
}
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
            if (infra.Dados != null)
            {
                string infraCargos = "";
                infra.Dados.Cargos.ForEach(x => infraCargos += $"<&@{x.Value}>, ");
                string infraDados = $"Cargos : {(infra.Dados.Cargos == null ? "Sem Dados" : $"{infraCargos}")}\n" +
                $"Tempo : {(String.IsNullOrWhiteSpace(infra.Dados.Tempo) ? "Sem Dados" : $"{infra.Dados.Tempo}")}";
                builder.AddField("Dados", infraDados);
            }
            return builder.Build();
        }

        public static async Task<DiscordEmbed> Censo(CensoModel dados)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            DiscordUser user = await Bot.Instance().client.GetUserAsync(dados.DiscordId);
            builder
            .WithColor(DiscordColor.Goldenrod)

            .WithDescription($"Censo Feito em : {TimeZoneInfo.ConvertTimeFromUtc(dados.Timestamp,TimeZoneInfo.FindSystemTimeZoneById("Brazil/East")).ToString(new CultureInfo("pt-BR"))}")
            .AddField("Idiomas",dados.Idiomas,true)
            .AddField("Jogos",dados.Jogos,true)
            .AddField("Origem",dados.Origem,true)
            .AddField("UF",dados.UF.ToString(),true)
            .AddField("Email",dados.Email,true)
            .AddField("Idade",dados.Idade.ToString(),true)
            .WithAuthor($"Censo do(a) {user.Username}#{user.Discriminator}",null,user.AvatarUrl);
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
            .AddField("Numero de Infrações",infracoes.Count.ToString(),true)
            .AddField("Cargo",profile.Cargo.ToString(),true)
            .WithColor(DiscordColor.Blurple)
            .WithAuthor($"Perfil de {user.Username}#{user.Discriminator}",null,user.AvatarUrl);
            if(infracoes.Count > 0)
            {
                foreach(var infra in infracoes)
                {
                    InfraStr += $"`#{infra.Id}` - `{infra.MotivoInfracao} (`<@{infra.IdStaff}>`)`\n";
                }
            }
            else
            {
                InfraStr += "`Nenhuma Infração.`";
            }
            builder.AddField("Infrações",InfraStr);
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

    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
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
        public static async Task<DiscordEmbed> MemberBriefing(List<InfracaoModel> infraList)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            DiscordUser user = await Bot.Instance().client.GetUserAsync(infraList[0].IdInfrator);
            string data = "";
            infraList.ForEach(x => data += $"`[#{x.Id}]` - `{x.MotivoInfracao} (`<@{x.IdStaff}>`)`\n");
            builder
            .WithDescription(data)
            .WithAuthor($"Infrações de {user.Username}#{user.Discriminator}",null,user.AvatarUrl);
            return builder.Build();
        }

    }
}
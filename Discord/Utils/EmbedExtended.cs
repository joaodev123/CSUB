using System;
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
    }
}
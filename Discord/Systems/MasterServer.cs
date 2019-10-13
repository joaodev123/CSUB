using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
namespace Discord.Systems
{
    public class MasterServer : IApplyToClient, IApplicableSystem
    {
        public string Name {get;set;}
        public string Description {get;set;}
        public bool Active {get;set;}

        public void Activate()
        {
            Name = "MasterServer";
            Description = "Previne o uso de comandos em servidores não autorizados";
            Active = true;
        }

        public void ApplyToClient(DiscordClient client)
        {
            client.GuildDownloadCompleted += async delegate (GuildDownloadCompletedEventArgs args)
            {

                if (Bot.Instance().cfg.MasterId == 0)
                {
                    if (client.Guilds.Count > 1)
                    {
                        var names = client.Guilds.Aggregate("", (current, guild) => current + $"{guild.Value.Name}, ");

                        foreach (KeyValuePair<ulong, DiscordGuild> guild in client.Guilds)
                        {
                            var ch = guild.Value.GetDefaultChannel();
                            var builder = new DiscordEmbedBuilder();

                            builder
                                .AddField("List of Servers", names)
                                .WithDescription($"Bot is loaded on multiple servers, please use {client.CurrentUser.Mention}setMaster on the MASTER guild.")
                                .WithAuthor("Error on determining MASTER server")
                                .WithColor(DiscordColor.Red);
                            await ch.SendMessageAsync(embed: builder);
                        }

                    }
                    else
                    {
                        var ch = client.Guilds.Values.ToList()[0].GetDefaultChannel();
                        var builder = new DiscordEmbedBuilder();
                        builder
                            .WithAuthor("Guild set as MASTER guild")
                            .WithColor(DiscordColor.PhthaloGreen);
                        Bot.Instance().cfg.MasterId = client.Guilds.Values.ToList()[0].Id;
                        File.WriteAllText(Directory.GetCurrentDirectory() + "Config.json", JsonConvert.SerializeObject(Bot.Instance().cfg, Formatting.Indented));
                    }
                }
            };
        }


        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }
    }
}

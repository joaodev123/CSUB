using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Utils;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Logic;
using Logic.Models;
using DSharpPlus.Entities;

namespace Discord.Systems
{
    public class Prisao : IApplicableSystem, IApplyToClient
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        private System.Threading.Timer timer { get; set; }
        public void Activate()
        {
            Active = true;
            Name = "Prisioneiro";
            Description = "Sistema de Infração do Discord : Prisioneiros";

        }

        private async Task Check()
        {
            Console.WriteLine("[Check @ Systems/Prisao.cs] Executing...");
            var activePrisao = new Logic.Prisao().FindAll(x => x.Elapsed == false);
            foreach (var p in activePrisao)
            {
                if (p.Data + p.Duracao > DateTime.Now)
                {
                    p.Elapsed = true;
                    await Soltar(p);
                    new Logic.Prisao().Update(x => x.Id == p.Id, p);
                }
            }
            Console.WriteLine("[Check @ Systems/Prisao.cs] Completed.");
        }

        private async Task Soltar(PrisaoModel p)
        {
            var guild = await Bot.Instance().client.GetGuildAsync(p.GuildId);
            var member = await guild.GetMemberAsync(new Infracao().Find(x => x.Id == p.InfraId).IdInfrator);
            await member.RevokeRoleAsync(member.Roles.ToList()[0]);
            foreach (var role in p.Cargos)
            {
                await member.GrantRoleAsync(guild.GetRole(role));
            }
            Console.WriteLine($"[Prisao] {member.Username}#{member.Discriminator} Solto.");
        }

        public void ApplyToClient(DiscordClient client)
        {
            if (Active)
            {
                client.GuildMemberAdded += MembroEntra;
                client.GuildDownloadCompleted += GuildDone;
                client.SocketClosed += Disconnect;
            }
        }

#pragma warning disable CS1998
        private async Task Disconnect(SocketCloseEventArgs e)
        {
            timer.Dispose();
            Console.WriteLine("[Timer] Disposed due to socket closed.");
        }

        private async Task GuildDone(GuildDownloadCompletedEventArgs e)
        {
            timer = new System.Threading.Timer(
                async e => await Check(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10)
            );
            Console.WriteLine("[Timer] Created.");
        }

        private async Task MembroEntra(GuildMemberAddEventArgs e)
        {
            var infra = new Infracao().FindAll(x => x.IdInfrator == e.Member.Id).Where(x => x.Preso).ToList();
            if (infra.Any(x => new Logic.Prisao().Find(y => y.InfraId == x.Id && y.Elapsed == false) != null))
            {
                DiscordRole r = e.Guild.GetRole(Roles.PresoID);
                await e.Member.GrantRoleAsync(r, "Tentou Evadir Prisão");
                await e.Guild.GetChannel(Channels.StaffID).SendMessageAsync("@here", false, EmbedBase.OutputEmbed($"Atenção > O Membro {e.Member.Mention} tentou evadir uma prisão ativa!"));
            }
        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }
    }
}
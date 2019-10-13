using System.Linq;
using System;
using System.Threading.Tasks;
using Discord.Utils;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Logic;
using DSharpPlus.Entities;

namespace Discord.Systems
{
    public class Prisao : IApplicableSystem, IApplyToClient
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public void Activate()
        {
            Active = true;
            Name = "Prisioneiro";
            Description = "Sistema de Infração do Discord : Prisioneiros";
        }

        public void ApplyToClient(DiscordClient client)
        {
            if (Active)
            {
                client.GuildMemberAdded += MembroEntra;
            }
        }

        private async Task MembroEntra(GuildMemberAddEventArgs e)
        {
            var infra = Infracao.FindAllInfracao(x => x.IdInfrator == e.Member.Id).Where(x => x.Preso);
            if (infra != null)
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
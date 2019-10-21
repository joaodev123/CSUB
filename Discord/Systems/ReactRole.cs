using System;
using System.Threading.Tasks;
using Discord.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Logic;

namespace Discord.Systems
{
    public class ReactRole : IApplicableSystem, IApplyToClient
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public void Activate()
        {
            this.Name = "ReactRole";
            this.Description = "Sistema de ReactRole";
            this.Active = true;
        }

        public void ApplyToClient(DiscordClient client)
        {
            if (this.Active)
            {
                client.MessageReactionAdded += ReactionAdded;
                client.MessageReactionRemoved += ReactionRemoved;
            }
        }

        private async Task ReactionRemoved(MessageReactionRemoveEventArgs e)
        {
            var Channel = new ReactChannel().Find(x => x.DiscordID == e.Channel.Id);
            if (Channel != null)
            {
                foreach (var i in Channel.Categories)
                {
                    var category = new ReactCategory().Find(x => x.Id == i);
                    if (category.MessageId == e.Message.Id)
                    {
                        foreach (var id in category.Roles)
                        {
                            var role = new Logic.ReactRole().Find(x => x.Id == id);
                            if (role.EmojiId == e.Emoji.Id)
                            {
                                await ((DiscordMember)e.User).RevokeRoleAsync(e.Channel.Guild.GetRole(role.RoleId));
                            }
                        }
                    }
                }
            }
        }

        private async Task ReactionAdded(MessageReactionAddEventArgs e)
        {
            var Channel = new ReactChannel().Find(x => x.DiscordID == e.Channel.Id);
            if (Channel != null)
            {
                foreach (var i in Channel.Categories)
                {
                    var category = new ReactCategory().Find(x => x.Id == i);
                    if (category.MessageId == e.Message.Id)
                    {
                        foreach (var id in category.Roles)
                        {
                            var role = new Logic.ReactRole().Find(x => x.Id == id);
                            if (role.EmojiId == e.Emoji.Id)
                            {
                                await ((DiscordMember)e.User).GrantRoleAsync(e.Channel.Guild.GetRole(role.RoleId));
                            }
                        }
                    }
                }
            }
        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }
    }
}
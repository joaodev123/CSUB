using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Attributes;
using Discord.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;

namespace Discord.Commands
{
    public class Developer : BaseCommandModule
    {
        [Command("setMaster")]
        [RequireOwner]
        public async Task SetMaster(CommandContext ctx)
        {
            List<DiscordGuild> guilds = Bot.Instance().client.Guilds.Values.ToList();
            var ids = new List<ulong?>();
            var embed = new DiscordEmbedBuilder();
            embed
                .WithAuthor($"Set {ctx.Guild.Name} as MASTER")
                .WithColor(DiscordColor.PhthaloGreen);

            guilds.Remove(ctx.Guild);
            Bot.Instance().cfg.MasterId = ctx.Guild.Id;
            guilds.ForEach(x => ids.Add(x.Id));
            Bot.Instance().cfg.PuppetId = ids;
            File.WriteAllText(Program.filedir,
                JsonConvert.SerializeObject(Bot.Instance().cfg, Formatting.Indented));
            await ctx.RespondAsync(embed: embed);
        }
    }


    [OniiSan,Imouto,Emoji("💻"),Group("system")]
    [DSharpPlus.CommandsNext.Attributes.Description("This group configures the systems.")]
    public class System : BaseCommandModule
    {
        [GroupCommand]
        public async Task Command(CommandContext ctx)
        {
            var embed = EmbedBase.GroupHelpEmbed(ctx.Command);
            await ctx.RespondAsync(embed: embed);
        }

        [Command("list")]
        [DSharpPlus.CommandsNext.Attributes.Description("Lists all active systems")]
        public async Task List(CommandContext ctx)
        {
            var allSystems = new List<string>();
            Bot.Instance().systems.ForEach(x => allSystems.Add(x.Name));
            var embed = EmbedBase.ListEmbed(allSystems, "Systems");
            await ctx.RespondAsync(embed: embed);
        }
    }
    [OniiSan] // Sets group to be only executable on the master server
    [Imouto] // Sets group to be only executable by the staff (Modify Roles)
    [Emoji(":wrench:")] // Sets the emoji for the group
    [Group("config")]
    [DSharpPlus.CommandsNext.Attributes.Description("This group configures the bot.")]
    public class Config : BaseCommandModule
    {
        [GroupCommand]
        public async Task Command(CommandContext ctx)
        {
            var embed = EmbedBase.GroupHelpEmbed(ctx.Command);
            await ctx.RespondAsync(embed: embed);
        }



        [Command("prefix")]
        [DSharpPlus.CommandsNext.Attributes.Description("Changes the custom prefixes")]
        public async Task PrefixError(CommandContext ctx)
        {
            var embed = EmbedBase.CommandHelpEmbed(ctx.Command);
            await ctx.RespondAsync(embed: embed);
        }

        // Sample of a command that uses interactivity and lolibase to input/output text data.
        [Command("prefix")]
        public async Task PrefixSuccess(CommandContext ctx, [DSharpPlus.CommandsNext.Attributes.Description("The operation to be executed [add/list/del] ")]
            string operation)
        {
            switch (operation.ToLowerInvariant())
            {
                case "add":
                    var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Prefix to be added"));
                    InteractivityResult<DiscordMessage> response = await ctx.Message.GetNextMessageAsync();
                    var prefix = response.Result.Content;
                    if (Bot.Instance().cfg.Prefixes.Contains(prefix.ToLowerInvariant()))
                    {
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"This prefix is already added."));
                    }
                    else
                    {
                        Bot.Instance().cfg.Prefixes.Add(prefix.ToLowerInvariant());
                        File.WriteAllText(Program.filedir,
                            JsonConvert.SerializeObject(Bot.Instance().cfg, Formatting.Indented));
                        await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Prefix added without errors."));
                    }

                    break;
                case "del":
                    var msg2 = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Prefix to be removed"));
                    InteractivityResult<DiscordMessage> response2 = await ctx.Message.GetNextMessageAsync();
                    var prefix2 = response2.Result.Content;
                    if (!Bot.Instance().cfg.Prefixes.Contains(prefix2.ToLowerInvariant()))
                    {
                        await msg2.ModifyAsync(embed: EmbedBase.OutputEmbed($"This prefix doesn't exists."));
                    }
                    else
                    {
                        Bot.Instance().cfg.Prefixes.Remove(prefix2.ToLowerInvariant());
                        File.WriteAllText(Program.filedir,
                            JsonConvert.SerializeObject(Bot.Instance().cfg, Formatting.Indented));
                        await msg2.ModifyAsync(embed: EmbedBase.OutputEmbed($"Prefix removed without errors."));
                    }
                    break;
                case "list":
                    await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(Bot.Instance().cfg.Prefixes, "Prefixes"));
                    break;
                default:
                    var embed = EmbedBase.CommandHelpEmbed(ctx.Command);
                    await ctx.RespondAsync(embed: embed);
                    break;
            }
        }
    }
}
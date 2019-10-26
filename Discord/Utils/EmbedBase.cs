using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Attributes;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
namespace Discord.Utils
{
    //Lolibase is made by me, also this code is repetitive af.
    public static class EmbedBase
    {

        public static DiscordEmbed GroupHelpEmbed(Command Command, CommandContext ctx)
        {
            List<Command> commands = new List<Command>();
            CommandGroup cG = null;
            if (Command is CommandGroup cGroup)
            {
                commands = cGroup.Children.ToList();
                cG = cGroup;
            }
            var commandList = "";
            foreach (var command in commands)
            {
                var show = command.RunChecksAsync(ctx, true).GetAwaiter().GetResult();
                if (show.Count() == 0)
                {
                    commandList += $"{command.Name} - {command.Description}\n";
                }
            }
            var groupHelpEmbed = new DiscordEmbedBuilder();
            groupHelpEmbed
                .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                .WithDescription(cG?.Description)
                .AddField("Commands", string.IsNullOrWhiteSpace(commandList) ? "No sub-commands found" : commandList)
                .WithAuthor($"Group : {cG?.Name} | Help")
                .WithColor(DiscordColor.Gray);
            return groupHelpEmbed.Build();
        }

        public static DiscordEmbed HelpEmbed(this CommandsNextExtension cne, CommandContext ctx)
        {
            List<Command> x = cne.RegisteredCommands.Values.ToList();
            List<Command> delRange = new List<Command>();
            var groups = new List<CommandGroup>();
            foreach (var command in x)
            {
                if (command is CommandGroup group)
                {
                    var show = group.RunChecksAsync(ctx, true).GetAwaiter().GetResult();
                    if (show.Count() == 0)
                    {
                        groups.Add(group);
                    }
                    delRange.Add(command);
                }
            }
            delRange.ForEach(gx => x.Remove(gx));
            var helpBuilder = new DiscordEmbedBuilder();
            foreach (var commandGroup in groups)
            {

                List<Command> children = commandGroup.Children.ToList();
                foreach (var command in children)
                {
                    x.Remove(command);
                }

                x.Remove(commandGroup);
                List<Attribute> attributes = commandGroup.CustomAttributes.ToList();
                commandGroup.ExecutionChecks.ToList().ForEach(yp => attributes.Add(yp));
                attributes.Reverse();
                string Prefix = "";
                foreach (var y in attributes)
                {

                    if (y is ImoutoAttribute) Prefix += "`[妹]` ";
                    if (y is OniiSanAttribute) Prefix += "`[兄]` ";
                    if (y is EmojiAttribute emoji) helpBuilder.AddField($"{Prefix}{emoji.Emoji} ・ {commandGroup.Name}", commandGroup.Description);
                }

            }

            var misc = "";
            foreach (var command in x)
            {
                var show = command.RunChecksAsync(ctx, true).GetAwaiter().GetResult();
                if (show.Count() == 0)
                {
                    misc += $"`{command.Name}` ";
                }
            }

            helpBuilder.AddField("❓ ・ Miscellaneous ", misc);
            helpBuilder
                .WithDescription($"To see help for a group run {Bot.Instance().client.CurrentUser.Mention} `group name`")
                .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                .WithAuthor("Help | Showing all groups")
                .WithColor(DiscordColor.CornflowerBlue);
            return helpBuilder.Build();
        }

        public static DiscordEmbed InputEmbed(string input)
        {
            var inputEmbedBuilder = new DiscordEmbedBuilder();
            inputEmbedBuilder
                .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                .WithDescription($"Please type : {input}")
                .WithColor(DiscordColor.MidnightBlue);
            return inputEmbedBuilder.Build();
        }

        public static DiscordEmbed OutputEmbed(string output)
        {
            var outputEmbedBuilder = new DiscordEmbedBuilder();
            outputEmbedBuilder
                .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                .WithDescription($"{output}")
                .WithColor(DiscordColor.SpringGreen);
            return outputEmbedBuilder.Build();
        }

        public static DiscordEmbed OrderedListEmbed<T>(List<T> list, string name)
        {
            var data = "";
            foreach (var item in list)
            {
                data += $"{list.IndexOf(item)}・{item.ToString()}\n";
            }
            var orderedListBuilder = new DiscordEmbedBuilder();
            orderedListBuilder
                .WithAuthor($"List of : {name}")
                .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                .WithDescription(string.IsNullOrWhiteSpace(data) ? "No data" : data)
                .WithColor(DiscordColor.Orange);
            return orderedListBuilder.Build();
        }

        public static DiscordEmbed ListEmbed<T>(IEnumerable<T> list, string name)
        {
            var data = list.Aggregate("", (current, item) => current + $"・{item.ToString()}\n");
            var listBuilder = new DiscordEmbedBuilder();
            listBuilder
                .WithAuthor($"List of : {name}")
                .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                .WithDescription(string.IsNullOrWhiteSpace(data) ? "No data" : data)
                .WithColor(DiscordColor.Orange);
            return listBuilder.Build();
        }
        public static DiscordEmbed CommandHelpEmbed(Command command)
        {
            if (command.Overloads?.Any() == true)
            {
                var use = "";
                List<CommandOverload> o = command.Overloads.ToList();
                var arguments = new List<CommandArgument>();
                o.RemoveAll(x => x.Arguments.Count == 0);
                foreach (var overload in o)
                {
                    var inner = "";
                    List<CommandArgument> args = overload.Arguments.ToList();
                    foreach (var argument in args)
                    {
                        if (!arguments.Contains(argument))
                        {
                            arguments.Add(argument);
                        }
                        inner += $"`{argument.Name}` ";
                    }
                    use += $"[{command.Name} {inner}] ";
                }

                var argumentExplanation = "";
                arguments.ForEach(x => argumentExplanation += $"{x.Name} - {x.Description}\n");
                var commandHelpEmbed = new DiscordEmbedBuilder();
                commandHelpEmbed
                    .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                    .AddField("Arguments", argumentExplanation)
                    .WithDescription($"Use : {use}")
                    .WithAuthor($"Command : {command.Name} | Help")
                    .WithColor(DiscordColor.Gray);
                return commandHelpEmbed.Build();
            }
            else
            {
                var commandHelpEmbed = new DiscordEmbedBuilder();
                commandHelpEmbed
                    .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                    .WithDescription("This command is a stub and was not implemented yet.")
                    .WithAuthor($"Command : {command.Name} | Help")
                    .WithColor(DiscordColor.Gray);
                return commandHelpEmbed.Build();
            }
        }
    }
}

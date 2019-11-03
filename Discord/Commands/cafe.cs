/*
 * File: cafe.cs
 * Project: Commands
 * File Created: Sunday, 3rd November 2019 7:48:39 pm
 * Author: RORIdev (t3ctotalmenterandom1@outlook.com)
 * -----
 * Copyright 2019 - RORIdev, lolidevs
 */

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.Commands
{
    public class cafe : BaseCommandModule
    {
        [Command("cafe")]
        public async Task cafeCommand(CommandContext ctx) => await ctx.RespondAsync("Quero café.");
    }
}
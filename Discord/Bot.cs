using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Utils;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using static System.Reflection.Assembly;
namespace Discord
{
    public class Bot
    {
        private static Bot instance;
        public DiscordClient client;
        public List<Type> systems = new List<Type>();
        private DiscordConfiguration config;
        public Config cfg;
        public CommandsNextExtension cnext;
        public InteractivityExtension interactivity;

        private Bot(DiscordConfiguration config)
        {
            this.config = config;
            this.client = new DiscordClient(this.config);
            this.cfg = null;
            this.systems = GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IApplicableSystem))).ToList();
            foreach (var system in systems)
            {

                if (system.GetInterfaces().Contains(typeof(IApplyToClient)))
                {
                    var instance = (IApplyToClient)Activator.CreateInstance(system);
                    instance.Activate();
                    instance.ApplyToClient(this.client);
                    Console.WriteLine($"[System] {instance.Name} Loaded\n\t{instance.Description}");
                }
            }
            this.cnext = this.client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolver,
                EnableDefaultHelp = false,
                EnableDms = true
            });
            this.interactivity = this.client.UseInteractivity(new InteractivityConfiguration{
                Timeout = TimeSpan.FromMinutes(30)
            });
        }

        private Bot(Config config)
        {
            this.config = new DiscordConfiguration
            {
                Token = config.Token
            };
            this.client = new DiscordClient(this.config);
            this.cfg = config;
            this.systems = GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IApplicableSystem))).ToList();
            this.systems = GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IApplicableSystem))).ToList();
            foreach (var system in systems)
            {

                if (system.GetInterfaces().Contains(typeof(IApplyToClient)))
                {
                    var instance = (IApplyToClient)Activator.CreateInstance(system);
                    instance.Activate();
                    instance.ApplyToClient(this.client);
                    Console.WriteLine($"[System] {instance.Name} Loaded\n\t{instance.Description}");
                }
            }

            this.cnext = this.client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolver,
                EnableDefaultHelp = false,
                EnableDms = true
            });
            this.interactivity = this.client.UseInteractivity(new InteractivityConfiguration{
                Timeout = TimeSpan.FromMinutes(30)
            });
        }

        #pragma warning disable CS1998
        private async Task<int> PrefixResolver(DiscordMessage msg)
        {
            switch (msg.GetMentionPrefixLength(this.client.CurrentUser))
            {
                case -1:
                    int x;
                    foreach (var prefix in this.cfg.Prefixes)
                    {
                        x = msg.GetStringPrefixLength(prefix);
                        if (x != -1)
                            return x;
                    }

                    break;
                default:
                    return msg.GetMentionPrefixLength(this.client.CurrentUser);
            }

            return -1;
        }

        public static Bot Instance(DiscordConfiguration config)
        {
            if (instance == null) instance = new Bot(config);
            return instance;
        }


        public static Bot Instance(Config config)
        {
            if (instance == null) instance = new Bot(config);
            return instance;
        }
        public static Bot Instance()
        {
            return instance;
        }
    }
}
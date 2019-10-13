using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using DSharpPlus;
using static System.Reflection.Assembly;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    class Program
    {
        private static Bot Bot { get; set; }
        public static string filedir { get; set; }
        static void Main(string[] args)
        {
            Config c;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) //Makes shit common for all of us.
            {
                Program.filedir = Directory.GetCurrentDirectory() + "/Config.json";
                if (File.Exists(Directory.GetCurrentDirectory() + "/Config.json"))
                {
                    c = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directory.GetCurrentDirectory()));
                }
                else
                {
                    c = CLIConfig(Environment.OSVersion.Platform);
                }
            }
            else
            {
                Program.filedir = Environment.GetEnvironmentVariable("HOME") + "/.config/CSUB/botrc";

                if (File.Exists(Environment.GetEnvironmentVariable("HOME") + "/.config/CSUB/botrc"))
                {
                    c = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Environment.GetEnvironmentVariable("HOME") + "/.config/CSUB/botrc"));
                }
                else
                {
                    c = CLIConfig(Environment.OSVersion.Platform);
                }
            }
            Bot = Bot.Instance(c);
            Bot.cnext.RegisterCommands(GetExecutingAssembly());
            Init().GetAwaiter().GetResult();
        }
        private static async Task Init()
        {
            await Bot.client.ConnectAsync();
            await Task.Delay(-1);
        }
        private static Config CLIConfig(PlatformID platform)
        {
            Config ic = new Config();
            if (platform == PlatformID.Win32NT)
            {
                Program.filedir = Directory.GetCurrentDirectory() + "/Config.json";
                Console.Clear();
                Console.WriteLine("Welcome to the configuration Screen.");
                Console.WriteLine($"OS > {platform.ToString()}");
                Console.WriteLine("\n\n\nPlease Type the Token:\n");
                ic.Token = Console.ReadLine();
                Console.WriteLine("\n\n\nConfiguration Completed.");
                Console.WriteLine($"File Saved At : {Program.filedir}");
                File.WriteAllText(Program.filedir, JsonConvert.SerializeObject(ic, Formatting.Indented));
            }
            else
            {
                Program.filedir = Environment.GetEnvironmentVariable("HOME") + "/.config/CSUB/botrc";
                Console.Clear();
                Console.WriteLine("Welcome to the configuration Screen.");
                Console.WriteLine($"OS > {platform.ToString()}");
                Console.WriteLine("\n\n\nPlease Type the Token:\n");
                ic.Token = Console.ReadLine();
                Console.WriteLine("\n\n\nConfiguration Completed.");
                Console.WriteLine($"File Saved At : {Program.filedir}");
                Directory.CreateDirectory(Environment.GetEnvironmentVariable("HOME") + "/.config/CSUB");
                File.WriteAllText(Program.filedir, JsonConvert.SerializeObject(ic, Formatting.Indented));
            }
            return ic;
        }
    }
}
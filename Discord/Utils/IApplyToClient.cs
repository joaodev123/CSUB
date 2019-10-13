
using DSharpPlus;

namespace Discord.Utils
{

    public interface IApplyToClient
    {
        string Name {get;set;}
        string Description {get;set;}
        bool Active {get;set;}
        void ApplyToClient(DiscordClient client);
        void Activate();
        void Deactivate();
    }
}
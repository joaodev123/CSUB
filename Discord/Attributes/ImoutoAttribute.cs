using System.Linq;

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.Attributes
{
    public class ImoutoAttribute : CheckBaseAttribute
    {
        #pragma warning disable CS1998
        public async override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return ctx.Member.Roles.Any(x => x.Permissions.HasPermission(Permissions.ManageRoles));
        }

    }
}

using System.Collections.Generic;

namespace Logic.Models
{
    public class ReactChannelModel
    {
        public int Id { get; set; }
        public ulong DiscordID { get; set; }
        public List<int> Categories { get; set; }
    }
}
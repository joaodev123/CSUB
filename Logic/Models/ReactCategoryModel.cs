using System.Collections.Generic;

namespace Logic.Models
{
    public class ReactCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong ChannelId {get;set;}
        public ulong MessageId { get; set; }
        public string Description { get; set; }
        public List<int> Roles { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Logic.Models
{
    public class PrisaoModel
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Duracao { get; set; }
        public List<ulong> Cargos { get; set; }
        public int InfraId { get; set; }
        public ulong GuildId { get; set; }
        public bool Elapsed { get; set; }
    }
}
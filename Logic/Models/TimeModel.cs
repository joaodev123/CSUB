using System.Collections.Generic;

namespace Logic.Models
{
    public class TimeModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ulong LiderId { get; set; }
        public List<ulong> Jogadores { get; set; }
        public List<ulong> Reservas { get; set; }
    }
}
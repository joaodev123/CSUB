using System.Collections.Generic;

namespace Logic.Models
{
    public class InfracaoModel
    {
        public int Id { get; internal set; }
        public ulong IdInfrator { get; set; }
        public ulong IdStaff { get; set; }
        public string MotivoInfracao { get; set; }
        public bool Preso { get; set; }
        public int PrisaoId { get; set; }
    }

}
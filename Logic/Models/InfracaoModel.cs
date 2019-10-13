using System.Collections.Generic;

namespace Logic.Models
{
    public class InfracaoModel
    {
        public int Id {get;internal set;}
        public ulong IdInfrator { get; set; }
        public ulong IdStaff { get; set; }
        public string MotivoInfracao { get; set; }
        public bool Preso { get; set; }
        public DadosPrisao Dados { get; set; }
    }

    public class DadosPrisao
    {
        public string Tempo { get; set; }
        public List<ulong?> Cargos { get; set; }
    }
}
using System.Collections.Generic;
namespace Logic.Models
{
    public class MembroModel
    {
        public ulong Id { get; set; }
        public bool Staff { get; set; }
        public List<int> Infracoes { get; set; }
        public Cargo Cargo { get; set; }

        public CensoModel Censo { get; set; }

    }
    public enum Cargo
    {
        DIRETOR_COMUNITARIO=1,
        ADMINISTRADOR=2,
        AJUDANTE_COMUNITARIO=4,
        SECRETARIO=8,
        MEMBRO_REGISTRADO=16,
        MEMBRO=0
    }
}
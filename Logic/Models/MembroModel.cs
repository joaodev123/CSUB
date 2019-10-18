using System.Collections.Generic;
namespace Logic.Models
{
    public class MembroModel
    {
        public int Id { get; set; }
        public ulong DiscordId {get;set;}
        public bool Staff { get; set; }
        public List<int> Infracoes { get; set; }
        public Cargo Cargo { get; set; }

        public int Censo { get; set; }

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
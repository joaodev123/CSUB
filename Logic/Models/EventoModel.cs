using System.Collections.Generic;

namespace Logic.Models
{
    public class EventoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<int> Times {get;set;}
        public int LimiteTimes {get;set;}
        public int LimiteJogadores {get;set;}
        public int LimiteReservas { get; set; }
    }
}
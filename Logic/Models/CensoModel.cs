using System;

namespace Logic.Models
{
    public class CensoModel
    {
        public DateTime Timestamp { get; set; }
        public string Email { get; set; }
        public string Origem { get; set; } //ChegouAUBGE -> Origem
        public int Idade { get; set; }
        public string Jogos { get; set; }
        public Estado UF { get; set; }
        public string Idiomas {get;set;}
    }
    public enum Estado
    {
        OTHER, AC, AL, AM, AP, BA, CE, DF, ES, GO, MA, MG, MS, MT, PA, PB, PE, PI, PR, RJ, RN, RO, RR, RS, SC, SE, SP, TO
    }
}
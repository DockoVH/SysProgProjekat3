using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Restoran
    {
        public string? Ime { get; set; }
        public double ProsecnaOcena { get; set; }
        public int BrojRecenzija { get; set; }
        public int Cena { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"\tIme: {Ime}");
            sb.AppendLine($"\tProsecna ocena: {ProsecnaOcena}");
            sb.AppendLine($"\tBroj recenzija: {BrojRecenzija}");
            sb.AppendLine($"\tCena: {new String('$', Cena)}");

            return sb.ToString();
        }
    }
}

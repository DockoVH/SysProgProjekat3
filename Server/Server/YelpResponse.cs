using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class YelpResponse
    {
        public int StatusCode { get; set; }
        public string Poruka { get; set; }
        public List<Restoran>? Restorani { get; set; } = new();

        public YelpResponse(IEnumerable<Restoran> restorani)
        {
            this.StatusCode = 200;
            Poruka = "";
            Restorani.AddRange(restorani);
        }

        public YelpResponse(string poruka, int statusCode = 400)
        {
            this.StatusCode = statusCode;
            Poruka = poruka;
            Restorani = null;
        }
    }
}

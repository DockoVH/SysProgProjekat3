using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace Server;

internal class RestoranProvider
{
    private readonly HttpClient client;
    private const string API_KEY = "";
    private const double minRating = 4.0;
    private const int minRecenzija = 500;

    public RestoranProvider()
    {
        client = new();
    }

    public async Task<YelpResponse> PribaviRestoraneAsync(string lokacija)
    {
        string url = $"https://api.yelp.com/v3/businesses/search?location={lokacija}&term=food&price=4&price=3&price=2&price=1";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);

        var response = await client.GetAsync(url);
        int statusCode = (int)response.StatusCode;
        var content = await response.Content.ReadAsStringAsync();
        var responseJSON = JObject.Parse(content);

        if(statusCode != 200)
        {
            var poruka = responseJSON["error"]!["description"]?.ToString() ?? "Nepoznata greška.";
            return new YelpResponse(poruka, statusCode);
        }

        var restoraniJSON = responseJSON["businesses"];

        if(restoraniJSON == null)
        {
            return new YelpResponse("Greška prilikom prikupljanja restorana.");
        }

        IEnumerable<Restoran> restorani =  restoraniJSON.Select(p => new Restoran()
        {
            Ime = (string?)p["name"] ?? "",
            BrojRecenzija = (int)p["review_count"]!,
            Cena = ((string?)p["price"] ?? "").Length,
            ProsecnaOcena = (double)p["rating"]!
        })
        .Where(p => p.ProsecnaOcena > minRating && p.BrojRecenzija > minRecenzija)
        .OrderByDescending(p => p.Cena);

        return new YelpResponse(restorani);
    }
}

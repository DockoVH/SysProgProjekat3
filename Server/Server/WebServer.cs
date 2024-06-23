using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Server;

internal class WebServer
{
    private readonly HttpListener listener = new();
    private const int port = 5050;

    public WebServer()
    {
        listener.Prefixes.Add($"http://localhost:{port}/");
    }

    public void Start()
    {
        try
        {
            listener.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška prilikom pokretanja servera: {ex.Message}");
            return;
        }

        Console.WriteLine($"Server pokrenut na adresi: http://localhost:{port}/");

        Task.Run(async () =>
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    await ObradiZahtevAsync(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Greška pri obradi zahteva: {ex.Message}");
                }
            }
        });
    }

    private async Task ObradiZahtevAsync(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        if(request.Url?.LocalPath.Length < 2)
        {
            await PosaljiOdgovorAsync(response, "Unesite ime grada", 400);
            return;
        }
        string lokacija = request.Url!.LocalPath.Substring(1);

        var scheduler = new NewThreadScheduler();
        var observer = new RestoranObserver("Restoran Observer");
        var provider = new RestoranProvider();
        RestoranSubject subject;
        IDisposable? sub;

        try
        {
            var odgovor = await provider.PribaviRestoraneAsync(lokacija);
            if (odgovor.StatusCode != 200)
            {
                await PosaljiOdgovorAsync(response, $"Greška {odgovor.StatusCode}: {odgovor.Poruka}", odgovor.StatusCode);
                Console.WriteLine($"[T:{Environment.CurrentManagedThreadId}]: Greška pri obradi zahteva: {request.Url}:");
                Console.WriteLine($"\tGreška {odgovor.StatusCode}: {odgovor.Poruka}");
                return;
            }

            List<Restoran> restorani = odgovor.Restorani!.ToList();
            subject = new RestoranSubject(restorani, scheduler);
            sub = subject.Subscribe(observer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška pri inicijalizaciji: {ex.Message}");
            await PosaljiOdgovorAsync(response, "Interna greška servera.", 500);
            return;
        }

        try
        {
            await observer.CompletionTask;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Greška prilikom obrade restorana: {ex.Message}");
            await PosaljiOdgovorAsync(response, "Interna greška servera", 500);

            if(sub != null)
            {
                sub.Dispose();
            }

            return;
        }

        if(sub != null)
        {
            sub.Dispose();
        }

        string content = JsonConvert.SerializeObject(observer.Restorani);

        await PosaljiOdgovorAsync(response, content, 200);
        Console.WriteLine($"[T:{Environment.CurrentManagedThreadId}]: Obradjen zahtev za: {request.Url}");
    }

    private async Task PosaljiOdgovorAsync(HttpListenerResponse response, string content, int statusCode = 200)
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";
        var buff = Encoding.UTF8.GetBytes(content);
        response.ContentLength64 = buff.Length;

        await response.OutputStream.WriteAsync(buff, 0, buff.Length);
        response.Close();
    }
}

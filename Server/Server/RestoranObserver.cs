using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class RestoranObserver : IObserver<Restoran>
    {
        private readonly string ime;
        public List<Restoran> Restorani { get; }
        private readonly TaskCompletionSource<bool> completionSource;

        public Task CompletionTask => completionSource.Task;

        public RestoranObserver(string ime)
        {
            this.ime = ime;
            Restorani = new();
            this.completionSource = new();
        }

        public void OnCompleted()
        {
            Console.WriteLine($"[T:{Thread.CurrentThread.ManagedThreadId}]: {ime}:\n\tPrikupljeni svi restorani\n");
            completionSource.SetResult(true);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"[T:{Thread.CurrentThread.ManagedThreadId}]: {ime}:\n\tGreška: {error.Message}\n");
            completionSource.SetException(error);
        }

        public void OnNext(Restoran value)
        {
            Console.WriteLine($"[T:{Thread.CurrentThread.ManagedThreadId}]: {ime}:\n{value.ToString()}\n");
            Restorani.Add(value);
        }
    }
}

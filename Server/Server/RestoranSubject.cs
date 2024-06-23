using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Server;

internal class RestoranSubject : IObservable<Restoran>
{
    private readonly IScheduler scheduler;
    private readonly IEnumerable<Restoran> restorani;

    public RestoranSubject(IEnumerable<Restoran> restorani, IScheduler? scheduler = null)
    {
        this.restorani = restorani;
        this.scheduler = scheduler ?? ImmediateScheduler.Instance;
    }

    public IDisposable Subscribe(IObserver<Restoran> observer)
    {
        return restorani.ToObservable<Restoran>()
        .ObserveOn(scheduler)
        .Subscribe(observer);
    }
}

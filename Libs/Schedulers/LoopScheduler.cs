using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Schedulers
{
  public class LoopScheduler : Runner, IProcessScheduler
  {
    /// <summary>
    /// Scheduler date
    /// </summary>
    public virtual DateTimeOffset Now => DateTime.Now;

    /// <summary>
    /// Scheduler
    /// </summary>
    public virtual EventLoopScheduler Instance { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public LoopScheduler()
    {
      Instance = new EventLoopScheduler(o => new Thread(o)
      {
        IsBackground = true,
        Priority = ThreadPriority.Highest,
        Name = nameof(LoopScheduler)
      });
    }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource Send(Action action)
    {
      var completion = new TaskCompletionSource();

      Instance?.Schedule(() =>
      {
        action();
        completion.TrySetResult();
      });

      return completion;
    }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource Send(Task action)
    {
      var completion = new TaskCompletionSource();

      Instance?.Schedule(() =>
      {
        action.GetAwaiter().GetResult();
        completion.TrySetResult();
      });

      return completion;
    }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource<T> Send<T>(Func<T> action)
    {
      var completion = new TaskCompletionSource<T>();

      Instance?.Schedule(() =>
      {
        completion.TrySetResult(action());
      });

      return completion;
    }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource<T> Send<T>(Task<T> action)
    {
      var completion = new TaskCompletionSource<T>();

      Instance?.Schedule(() =>
      {
        completion.TrySetResult(action.GetAwaiter().GetResult());
      });

      return completion;
    }

    /// <summary>
    /// Schedule wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="state"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual IDisposable Schedule<T>(T state, Func<IScheduler, T, IDisposable> action)
    {
      return Instance?.Schedule(state, action);
    }

    /// <summary>
    /// Schedule wrapper
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <param name="dueTime"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
      return Instance?.Schedule(state, dueTime, action);
    }

    /// <summary>
    /// Schedule wrapper
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <param name="dueTime"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
      return Instance?.Schedule(state, dueTime, action);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose() => Instance?.Dispose();
  }
}

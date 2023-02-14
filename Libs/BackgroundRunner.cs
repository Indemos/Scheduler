using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleSpace
{
  public class BackgroundRunner : IBaseRunner
  {
    /// <summary>
    /// Process
    /// </summary>
    protected Task _process;

    /// <summary>
    /// Queue
    /// </summary>
    protected BlockingCollection<Action> _actions = new(new ConcurrentQueue<Action>());

    /// <summary>
    /// Constructor
    /// </summary>
    public BackgroundRunner() : this(TaskScheduler.Default, CancellationToken.None)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="scheduler"></param>
    /// <param name="cancellation"></param>
    public BackgroundRunner(TaskScheduler scheduler, CancellationToken cancellation)
    {
      _process = Task.Factory.StartNew(Consume, cancellation, TaskCreationOptions.LongRunning, scheduler);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Dispose()
    {
      _process?.Dispose();
    }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource Send(Action action)
    {
      var source = new TaskCompletionSource();

      _actions.Add(() =>
      {
        action();
        source.TrySetResult();
      });

      return source;
    }

    /// <summary>
    /// Task processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource Send(Task action)
    {
      var source = new TaskCompletionSource();

      _actions.Add(() =>
      {
        action.GetAwaiter().GetResult();
        source.TrySetResult();
      });

      return source;
    }

    /// <summary>
    /// Delegate processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<T> Send<T>(Func<T> action)
    {
      var source = new TaskCompletionSource<T>();

      _actions.Add(() => source.TrySetResult(action()));

      return source;
    }

    /// <summary>
    /// Task processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<T> Send<T>(Task<T> action)
    {
      var source = new TaskCompletionSource<T>();

      _actions.Add(() => source.TrySetResult(action.GetAwaiter().GetResult()));

      return source;
    }

    /// <summary>
    /// Background process
    /// </summary>
    protected virtual void Consume()
    {
      foreach (var action in _actions.GetConsumingEnumerable())
      {
        try
        {
          action();
        }
        catch (Exception) {}
      }
    }
  }
}

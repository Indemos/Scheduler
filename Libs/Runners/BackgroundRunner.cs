using Schedule.EnumSpace;
using Schedule.ModelSpace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.RunnerSpace
{
  public class BackgroundRunner : Runner, IRunner
  {
    /// <summary>
    /// Process
    /// </summary>
    protected IList<Task> _processors;

    /// <summary>
    /// Wrapper
    /// </summary>
    protected BlockingCollection<ActionModel> _actions;

    /// <summary>
    /// Constructor
    /// </summary>
    public BackgroundRunner() : this(1, TaskScheduler.Default, CancellationToken.None)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public BackgroundRunner(int count) : this(count, TaskScheduler.Default, CancellationToken.None)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="count"></param>
    /// <param name="scheduler"></param>
    /// <param name="cancellation"></param>
    public BackgroundRunner(int count, TaskScheduler scheduler, CancellationToken cancellation)
    {
      _actions = new();
      _processors = Enumerable
        .Range(0, count)
        .Select(o => Task.Factory.StartNew(Consume, cancellation, TaskCreationOptions.LongRunning, scheduler))
        .ToList();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      _processors.ForEach(o => o.Dispose());
      _processors.Clear();
    }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource Send(Action action)
    {
      var source = new TaskCompletionSource();

      Enqueue(new ActionModel
      {
        Error = () => source.TrySetResult(),
        Success = () =>
        {
          action();
          source.TrySetResult();
        }
      });

      return source;
    }

    /// <summary>
    /// Task processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource Send(Task action)
    {
      var source = new TaskCompletionSource();

      Enqueue(new ActionModel
      {
        Error = () => source.TrySetResult(),
        Success = () =>
        {
          action.GetAwaiter().GetResult();
          source.TrySetResult();
        }
      });

      return source;
    }

    /// <summary>
    /// Delegate processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource<T> Send<T>(Func<T> action)
    {
      var source = new TaskCompletionSource<T>();

      Enqueue(new ActionModel
      {
        Error = () => source.TrySetResult(default),
        Success = () => source.TrySetResult(action())
      });

      return source;
    }

    /// <summary>
    /// Task processor
    /// </summary>
    /// <param name="action"></param>
    public override TaskCompletionSource<T> Send<T>(Task<T> action)
    {
      var source = new TaskCompletionSource<T>();

      Enqueue(new ActionModel
      {
        Error = () => source.TrySetResult(default),
        Success = () => source.TrySetResult(action.GetAwaiter().GetResult())
      });

      return source;
    }

    /// <summary>
    /// Enqueue
    /// </summary>
    /// <param name="action"></param>
    protected virtual void Enqueue(ActionModel item)
    {
      switch (Precedence)
      {
        case PrecedenceEnum.Input:

          if (_actions.Count >= Count - 1)
          {
            if (_actions.TryTake(out var o))
            {
              o.Error();
            }
          }

          _actions.TryAdd(item);

          break;

        case PrecedenceEnum.Process:

          if (_actions.Count >= Count)
          {
            item.Error();
          }

          if (_actions.Count < Count)
          {
            _actions.TryAdd(item);
          }

          break;
      }
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
          action.Success();

        } catch (Exception)
        {
          action.Error();
        }
      }
    }
  }
}

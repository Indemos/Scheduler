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
    /// Process controller
    /// </summary>
    protected CancellationTokenSource _controller;

    /// <summary>
    /// Process
    /// </summary>
    protected IList<Task> _processors;

    /// <summary>
    /// Wrapper
    /// </summary>
    protected BlockingCollection<ActionModel> _actions;

    /// <summary>
    /// Max number of processes in the queue
    /// </summary>
    public virtual int Count { get; set; }

    /// <summary>
    /// Priority that defines which process to handle first
    /// </summary>
    public virtual PrecedenceEnum Precedence { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public BackgroundRunner() : this(1, TaskScheduler.Default, CancellationToken.None)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public BackgroundRunner(int peocessors) : this(peocessors, TaskScheduler.Default, CancellationToken.None)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="processors"></param>
    /// <param name="scheduler"></param>
    /// <param name="cancellation"></param>
    public BackgroundRunner(int processors, TaskScheduler scheduler, CancellationToken cancellation)
    {
      _controller = new CancellationTokenSource();

      if (Equals(cancellation, CancellationToken.None))
      {
        cancellation = _controller.Token;
      }

      _actions = new();
      _processors = Enumerable
        .Range(0, processors)
        .Select(o => Task.Factory.StartNew(Consume, cancellation, TaskCreationOptions.LongRunning, scheduler))
        .ToList();

      Count = int.MaxValue;
      Precedence = PrecedenceEnum.Input;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      _controller.Cancel();
      _controller.Dispose();
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

          if (_actions.Count >= Count)
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
            return;
          }

          _actions.TryAdd(item);

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

        }
        catch (Exception)
        {
          action.Error();
        }
      }
    }
  }
}

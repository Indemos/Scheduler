using Schedule.EnumSpace;
using Schedule.ModelSpace;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule.RunnerSpace
{
  public class TimeRunner : Runner, IRunner
  {
    /// <summary>
    /// Updater
    /// </summary>
    protected DateTime _stamp;

    /// <summary>
    /// Wrapper
    /// </summary>
    protected ConcurrentQueue<ActionModel> _actions;

    /// <summary>
    /// Max number of actions in the queue
    /// </summary>
    public virtual int Count { get; set; }

    /// <summary>
    /// Execution delay
    /// </summary>
    public virtual TimeSpan Span { get; set; }

    /// <summary>
    /// Priority that defines which process to handle first
    /// </summary>
    public virtual PrecedenceEnum Precedence { get; set; }

    /// <summary>
    /// Time wrapper
    /// </summary>
    public virtual Func<DateTime> Time { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public TimeRunner()
    {
      _actions = new();
      _stamp = DateTime.MinValue;

      Count = int.MaxValue;
      Span = TimeSpan.Zero;
      Precedence = PrecedenceEnum.Input;
      Time = () => DateTime.UtcNow;
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
            if (_actions.TryDequeue(out var o))
            {
              o.Error();
            }
          }

          _actions.Enqueue(item);

          break;

        case PrecedenceEnum.Process:

          if (_actions.Count >= Count)
          {
            item.Error();
          }

          if (_actions.Count < Count)
          {
            _actions.Enqueue(item);
          }

          break;
      }

      var stamp = Time();

      if (stamp > (_stamp + Span))
      {
        _actions.ForEach(o => o.Success());
        _stamp = stamp;
      }
    }
  }
}

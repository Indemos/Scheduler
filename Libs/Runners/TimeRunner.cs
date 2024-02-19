using Schedule.EnumSpace;
using Schedule.ModelSpace;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule.Runners
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
      Precedence = PrecedenceEnum.Next;
      Time = () => DateTime.UtcNow;
    }

    /// <summary>
    /// Enqueue
    /// </summary>
    /// <param name="action"></param>
    protected override void Enqueue(ActionModel item)
    {
      switch (Precedence)
      {
        case PrecedenceEnum.Next:

          if (_actions.Count >= Count - 1)
          {
            if (_actions.TryDequeue(out var o))
            {
              o.Error(new ErrorModel
              {
                Code = ErrorEnum.Excess
              });
            }
          }

          _actions.Enqueue(item);

          break;

        case PrecedenceEnum.Current:

          if (_actions.Count >= Count)
          {
            item.Error(new ErrorModel
            {
              Code = ErrorEnum.Excess
            });
          }

          _actions.Enqueue(item);

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

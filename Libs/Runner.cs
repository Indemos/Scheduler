using Schedule.EnumSpace;
using System;
using System.Threading.Tasks;

namespace Schedule
{
  public interface IRunner : IDisposable
  {
    /// <summary>
    /// Max number of processes in the queue
    /// </summary>
    int Count { get; set; }

    /// <summary>
    /// Priority that defines which process to handle first
    /// </summary>
    PrecedenceEnum Precedence { get; set; }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource Send(Action action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource Send(Task action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource<T> Send<T>(Func<T> action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource<T> Send<T>(Task<T> action);
  }

  public abstract class Runner : IRunner
  {
    /// <summary>
    /// Max number of processes in the queue
    /// </summary>
    public virtual int Count { get; set; } = int.MaxValue;

    /// <summary>
    /// Priority that defines which process to handle first
    /// </summary>
    public virtual PrecedenceEnum Precedence { get; set; } = PrecedenceEnum.Input;

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose() => Count = int.MaxValue;

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public abstract TaskCompletionSource Send(Action action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public abstract TaskCompletionSource Send(Task action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public abstract TaskCompletionSource<T> Send<T>(Func<T> action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public abstract TaskCompletionSource<T> Send<T>(Task<T> action);
  }
}

using Schedule.EnumSpace;
using System;
using System.Threading.Tasks;

namespace Schedule
{
  public interface IRunner : IDisposable
  {
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
    /// Dispose
    /// </summary>
    public virtual void Dispose() { }

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

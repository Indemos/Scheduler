using System;
using System.Threading.Tasks;

namespace ScheduleSpace
{
  public interface IBaseRunner : IDisposable
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
}

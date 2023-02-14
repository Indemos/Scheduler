using System;
using System.Reactive.Concurrency;

namespace ScheduleSpace
{
  public interface IBaseScheduler : IScheduler, IBaseRunner, IDisposable
  {
  }
}

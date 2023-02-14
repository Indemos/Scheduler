using ScheduleSpace;
using System;
using System.Threading.Tasks;

namespace Tests
{
  public class Concurrency
  {
    [Fact]
    public void Scheduler()
    {
      var scheduler = new LoopScheduler();
      var x1 = Environment.CurrentManagedThreadId;
      var x2 = scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result;
      var x3 = Task.Run(() => scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result).Result;
      var x4 = Task.Factory.StartNew(() => scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result).Result;
      var x5 = Task.Run(async () => await scheduler.Send(() => Environment.CurrentManagedThreadId).Task).Result;

      Assert.NotEqual(x1, x2);
      Assert.Equal(x2, x3);
      Assert.Equal(x3, x4);
      Assert.Equal(x4, x5);
    }

    [Fact]
    public void Runner()
    {
      var scheduler = new BackgroundRunner();
      var x1 = Environment.CurrentManagedThreadId;
      var x2 = scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result;
      var x3 = Task.Run(() => scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result).Result;
      var x4 = Task.Factory.StartNew(() => scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result).Result;
      var x5 = Task.Run(async () => await scheduler.Send(() => Environment.CurrentManagedThreadId).Task).Result;

      Assert.NotEqual(x1, x2);
      Assert.Equal(x2, x3);
      Assert.Equal(x3, x4);
      Assert.Equal(x4, x5);
    }
  }
}

using Schedule.Schedulers;
using System;
using System.Threading.Tasks;

namespace Tests
{
  public class SchedulerTests
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

      Assert.NotEqual(x1, x2.Data);
      Assert.Equal(x2.Data, x3.Data);
      Assert.Equal(x3.Data, x4.Data);
      Assert.Equal(x4.Data, x5.Data);
    }
  }
}

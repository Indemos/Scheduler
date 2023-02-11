using ScheduleSpace;
using System;
using System.Threading.Tasks;

namespace Tests
{
  public class Concurrency
  {
    [Fact]
    public void SingleBackgroundProcess()
    {
      var scheduler = new MessageScheduler();
      var x1 = Environment.CurrentManagedThreadId;
      var x2 = scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result;
      var x3 = Task.Run(() => scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result).Result;

      Assert.NotEqual(x1, x2);
      Assert.Equal(x2, x3);
    }
  }
}

using Schedule.Runners;
using System;

namespace Tests
{
  public class TimeTests
  {
    [Fact]
    public void Run()
    {
      var count = 0;
      var scheduler = new TimeRunner
      {
        Count = 2,
        Span = TimeSpan.FromSeconds(1),
        Time = () => new DateTime(2000, 1, 1)
      };

      for (var i = 0; i < 5; i++)
      {
        scheduler.Send(() => count++);
      }

      Assert.Equal(1, count);
    }

    [Fact]
    public void UpdateTime()
    {
      var count = 0;
      var date = new DateTime(2000, 1, 1);
      var scheduler = new TimeRunner
      {
        Count = 2,
        Span = TimeSpan.FromSeconds(1),
        Time = () => date
      };

      for (var i = 0; i < 5; i++)
      {
        scheduler.Send(() => count++);
        scheduler.Time = () => date + TimeSpan.FromSeconds(5);
      }

      Assert.Equal(2, count);
    }
  }
}

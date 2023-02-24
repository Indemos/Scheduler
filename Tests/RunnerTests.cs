using Schedule.EnumSpace;
using Schedule.RunnerSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
  public class RunnerTests
  {
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

    [Fact]
    public void Access()
    {
      var exceptions = 0;
      var count = 1000000;
      var items = Enumerable.Range(1, count).ToList();
      var scheduler = new BackgroundRunner(Environment.ProcessorCount);

      try
      {
        Task.WaitAll(
          scheduler.Send(() => { for (var i = 0; i < count; i++) items.Add(i); }).Task,
          scheduler.Send(() => { for (var i = 1; i < count; i++) items[i] = items[i - 1] + items.Count; }).Task
        );
      }
      catch (Exception)
      {
        exceptions++;
      }

      Assert.Equal(0, exceptions);
    }

    [Fact]
    public void InputPriority()
    {
      var count = -1;
      var expectation = 10;
      var processes = new List<Task>();
      var scheduler = new BackgroundRunner(1) { Count = expectation };

      for (var i = 0; i < 50; i++)
      {
        processes.Add(scheduler.Send(() =>
        {
          Task.Delay(10).Wait();
          count++;

        }).Task);
      }

      Task.WaitAll(processes.ToArray());

      Assert.Equal(expectation, count);
    }

    [Fact]
    public void ProcessPriority()
    {
      var count = -1;
      var expectation = 10;
      var processes = new List<Task>();
      var scheduler = new BackgroundRunner(1) { Count = expectation, Precedence = PrecedenceEnum.Process };

      for (var i = 0; i < 50; i++)
      {
        processes.Add(scheduler.Send(() =>
        {
          Task.Delay(10).Wait();
          count++;

        }).Task);
      }

      Task.WaitAll(processes.ToArray());

      Assert.Equal(expectation, count);
    }
  }
}

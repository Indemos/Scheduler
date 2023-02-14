# Schedule

Schedulers for asynchronous execution in the main or background threads.

# Status

```
Install-Package Schedule
```

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/Indemos/Schedule/dotnet.yml?event=push)
![GitHub](https://img.shields.io/github/license/Indemos/Schedule)
![GitHub](https://img.shields.io/badge/system-Windows%20%7C%20Linux%20%7C%20Mac-blue)

# Sample 

```C#

// Guaranteed execution in a single thread for thread-safe access to context variables

var scheduler = new BackgroundRunner();
var x = scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result;
var y = Task.Run(() => scheduler.Send(() => Environment.CurrentManagedThreadId).Task.Result).Result;

Assert.Equal(x, y);

// Thread-safe single-threaded timer 

var num = new Random();
var collection = new List<int>();
var scheduler = new LoopScheduler();

Observable.Interval(span, scheduler).Subscribe(o => collection.Add(num.Next()));
```

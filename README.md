# Schedule

Schedulers for asynchronous execution in the main or background threads.

# Nuget

```
Install-Package Schedule
```

# Sample 

```C#

// Main thread

var x = 1;
var y = 1;
var scheduler = new MessageScheduler();

// Execution is performed in the background thread
// Response returns back to the main thread

var response = await scheduler.Send<int>(() => x + y).Task;
```
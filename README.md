# Schedule

Schedulers for asynchronous execution in the main or background threads.

# Nuget

```
Install-Package Schedule
```

# Sample 

```C#
var x = 1;
var y = 1;
var scheduler = new MessageScheduler();
var response = await scheduler.Send<int>(() => x + y).Task;
```
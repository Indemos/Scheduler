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

// Main thread

var x = 1;
var y = 1;
var scheduler = new MessageScheduler();

// Execution is performed in the background thread
// Response returns back to the main thread

var response = await scheduler.Send<int>(() => x + y).Task;
```

# Task 1
Here I was thinking I will make one function that triggers every minute and fetches the data and saves the results. And HTTP triggers that will serve as the API.


About 5 hours spent, quickly finished with main tasks, spent some time exploring the different options, api documentation etc.


Some challenges with consistency in this approach as something could happen between saving results to blob storage and table.
Calls to the service are not idempotent, so retries are not feasible.


A way to solve this could be to instead of doing two operations, do one, like publish a message containing all info on service bus.
Should consider potential sizes of payload for each scenario before going for that.
Another alternative blob trigger.
Too slow for this demo, event grid etc out of scope.


Spent some time looking at setting up logging, dependency injection etc.


Timestamps for queries, keeping it utc, not the most user friendly for a person, but a computer should have little issues.


Partition keys, rowkeys, performance, with more info could maybe made a better partitioning scheme


Pagination on response from get range, could be added, or a max range.


I chose to not add any tests for this and the second task, you will see that code is mostly testable with logical interfaces and dependency injection.


# Task 2
Requirements are confusing, I will just make something
About 6 hours spent, backend tasks were ok, frontend was a long time ago.


Use background service, get data, save in sql with entity framework.
If running multiple instances, need to synchronize updates.


Free public apis offering weatherdata by the minute is not that easy to find, current temperature was not current
rather it was hourly. Going with it. The weather provider is pluggable behind an interface, so easy to add an api that supports it.
If you can find it.


Frontend, just make some endpoints that expose operations specific to frontend needs and push the data to a chart framework.


The api is not in any way intended for use by anything else than the frontend code.


* Would normally do some time cutoffs, do queries in database, add indexes etc.
* Could also maintain read models tailored for the views for performance.
* Current solution will probably struggle with lot of data.
* Plan was ok, but im not a frontend developer.
* Probably something I misunderstood with the graphs part. Would have liked to ask for some clarifications.
* No error handling or logging added.

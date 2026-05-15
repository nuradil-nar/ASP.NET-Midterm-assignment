# Block 3 – Integration Pattern for NotificationService
# ============================================================

## Scenario
When `OnTaskCompleted` fires inside Task Service, a future `NotificationService`
must send an email to the task owner.

## Chosen Pattern: Asynchronous Messaging via RabbitMQ

### Why asynchronous?

1. **Decoupling** – Task Service does not need to know that a notification
   service exists. It simply publishes an event; any subscriber can react
   without changing the publisher.

2. **Resilience** – If the NotificationService is temporarily down, messages
   stay in the RabbitMQ queue and are delivered when it recovers.
   With synchronous HTTP/REST, a down NotificationService would cause the
   `PUT /complete` endpoint to fail or time out.

3. **Performance** – Sending an email can take hundreds of milliseconds
   (SMTP, external provider, etc.). Doing this synchronously in the HTTP
   request chain degrades API latency for the user.

4. **Scalability** – Multiple instances of NotificationService can consume
   from the same queue in parallel without any changes to Task Service.

## Technology: RabbitMQ + MassTransit

- **RabbitMQ** – battle-tested open-source message broker; easy to run in
  Docker alongside the microservice.
- **MassTransit** – .NET library that abstracts RabbitMQ (and others) behind
  a clean publish/subscribe API.

## Implementation sketch

### Task Service (Publisher)
```csharp
// In the OnTaskCompleted event handler (TasksController):
await _publishEndpoint.Publish(new TaskCompletedEvent
{
    TaskId    = task.Id,
    TaskTitle = task.Title,
    CompletedAt = DateTime.UtcNow
});
```

### NotificationService (Consumer)
```csharp
public class TaskCompletedConsumer : IConsumer<TaskCompletedEvent>
{
    public async Task Consume(ConsumeContext<TaskCompletedEvent> context)
    {
        var msg = context.Message;
        await _emailService.SendAsync(
            subject: $"Task '{msg.TaskTitle}' completed",
            body:    $"Task {msg.TaskId} was completed at {msg.CompletedAt}."
        );
    }
}
```

## Comparison with synchronous HTTP/REST

| Criterion         | Async (RabbitMQ)          | Sync (HTTP/REST)              |
|-------------------|---------------------------|-------------------------------|
| Coupling          | Loose                     | Tight                         |
| Failure handling  | Retry / dead-letter queue | Caller must handle 5xx        |
| Latency impact    | None (fire-and-forget)    | Adds email send time to API   |
| Complexity        | Higher (broker needed)    | Lower                         |
| Scalability       | High (many consumers)     | Limited by HTTP thread pool   |

## Conclusion
For a production microservice, **RabbitMQ + MassTransit** is the correct choice.
The slight operational overhead of running a broker is well worth the resilience
and decoupling benefits. HTTP/REST callbacks are acceptable only for simple
proofs-of-concept where operational simplicity outweighs reliability.

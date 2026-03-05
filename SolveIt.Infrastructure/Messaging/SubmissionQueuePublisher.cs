using SolveIt.Application.Interfaces;

namespace SolveIt.Infrastructure.Messaging;

public sealed class SubmissionQueuePublisher : ISubmissionQueuePublisher
{
    public Task PublishAsync(
        Guid submissionId,
        Guid problemId,
        CancellationToken cancellationToken)
    {
        // Temporary placeholder until Kafka/RabbitMQ worker is implemented
        return Task.CompletedTask;
    }
}

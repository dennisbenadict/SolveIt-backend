using MassTransit;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Messaging;

namespace SolveIt.Infrastructure.Messaging;

public sealed class SubmissionQueuePublisher : ISubmissionQueuePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public SubmissionQueuePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync(
        Guid submissionId,
        Guid problemId,
        CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new SubmissionRequested(submissionId, problemId),
            cancellationToken);
    }
}
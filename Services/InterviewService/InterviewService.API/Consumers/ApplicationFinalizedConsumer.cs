using InterviewingSystem.Contracts.IntegrationEvents;
using InterviewService.DAL.UoF;
using MassTransit;

namespace InterviewService.API.Consumers;

public class ApplicationFinalizedConsumer : IConsumer<ApplicationFinalizedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationFinalizedConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<ApplicationFinalizedEvent> context)
    {
        var cachedApplicationRepository = _unitOfWork.CachedApplicationRepository;

        var applicationId = context.Message.Id;

        await cachedApplicationRepository.DeleteAsync(applicationId);
        await _unitOfWork.SaveChangesAsync();
    }
}

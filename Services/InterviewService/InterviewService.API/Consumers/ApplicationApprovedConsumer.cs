using AutoMapper;
using DAL_Core.Entities;
using InterviewingSystem.Contracts.IntegrationEvents;
using InterviewService.DAL.UoF;
using MassTransit;

namespace InterviewService.API.Consumers;

public class ApplicationApprovedConsumer : IConsumer<ApplicationApprovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApplicationApprovedConsumer(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<ApplicationApprovedEvent> context)
    {
        var cachedApplicationRepository = _unitOfWork.CachedApplicationRepository;

        var dto = _mapper.Map<CachedApplication>(context.Message);
        dto.UpdatedAt = DateTime.UtcNow;

        await cachedApplicationRepository.CreateAsync(dto);
        await _unitOfWork.SaveChangesAsync();
    }
}

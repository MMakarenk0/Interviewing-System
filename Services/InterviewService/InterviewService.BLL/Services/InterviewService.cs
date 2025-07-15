using AutoMapper;
using DAL_Core.Entities;
using InterviewService.BLL.Clients;
using InterviewService.BLL.Models;
using InterviewService.BLL.Services.Interfaces;
using InterviewService.DAL.UoF;
using System.ComponentModel.DataAnnotations;

namespace InterviewService.BLL.Services;
public class InterviewService : IInterviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICandidateServiceApi _applicationClient;
    private const int ApplicationCacheDays = 1;

    public InterviewService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICandidateServiceApi applicationClient)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _applicationClient = applicationClient;
    }

    public async Task<Guid> CreateInterviewAsync(CreateInterviewDto model)
    {
        var interviewRepository = _unitOfWork.InterviewRepository;
        var slotRepository = _unitOfWork.InterviewSlotRepository;

        var existingInterview = await interviewRepository.GetAsync(model.Id);
        if (existingInterview != null)
            throw new ArgumentException($"Interview with ID={model.Id} already exists");

        var slot = await slotRepository.GetAsync(model.SlotId);
        if (slot == null)
            throw new ArgumentException($"Interview slot with ID={model.SlotId} not found");

        var application = await CheckAndRefreshApplicationAsync(model.ApplicationId);
        if (application == null)
            throw new ValidationException($"Application with ID={model.ApplicationId} not found or invalid");

        var interview = _mapper.Map<Interview>(model);

        await interviewRepository.CreateAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        return interview.Id;
    }

    public async Task<InterviewDto> GetInterviewByIdAsync(Guid id)
    {
        var interviewRepository = _unitOfWork.InterviewRepository;

        var interview = await interviewRepository.GetWithSlotAsync(id);
        if (interview == null)
            throw new ArgumentException($"Interview with ID={id} not found");

        return _mapper.Map<InterviewDto>(interview);
    }

    public async Task<IList<InterviewDto>> GetAllInterviewsAsync()
    {
        var interviewRepository = _unitOfWork.InterviewRepository;

        var interviews = await interviewRepository.GetAllWithSlotAsync();

        return _mapper.Map<IList<InterviewDto>>(interviews);
    }

    public async Task<Guid> UpdateInterviewAsync(CreateInterviewDto model)
    {
        var interviewRepository = _unitOfWork.InterviewRepository;
        var slotRepository = _unitOfWork.InterviewSlotRepository;

        var interview = await interviewRepository.GetAsync(model.Id);
        if (interview == null)
            throw new ArgumentException($"Interview with ID={model.Id} not found");

        var slot = await slotRepository.GetAsync(model.SlotId);
        if (slot == null)
            throw new ArgumentException($"Interview slot with ID={model.SlotId} not found");

        var application = await CheckAndRefreshApplicationAsync(model.ApplicationId);
        if (application == null)
            throw new ValidationException($"Application with ID={model.ApplicationId} not found or invalid");

        _mapper.Map(model, interview);

        await interviewRepository.UpdateAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        return interview.Id;
    }

    public async Task DeleteInterviewAsync(Guid id)
    {
        var interviewRepository = _unitOfWork.InterviewRepository;

        var interview = await interviewRepository.GetAsync(id);
        if (interview == null)
            throw new ArgumentException($"Interview with ID={id} not found");

        await interviewRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsInterviewOwnedByUserAsync(Guid interviewId, Guid userId)
    {
        var interviewRepository = _unitOfWork.InterviewRepository;

        var interview = await interviewRepository.GetWithSlotAsync(interviewId);

        return interview != null && interview.Slot.InterviewerId == userId;
    }

    private async Task<CachedApplication?> CheckAndRefreshApplicationAsync(Guid applicationId)
    {
        var appCacheRepo = _unitOfWork.CachedApplicationRepository;
        var cached = await appCacheRepo.GetAsync(applicationId);

        bool isStale = cached == null || cached.UpdatedAt < DateTime.UtcNow.AddDays(-ApplicationCacheDays);

        if (cached == null || isStale)
        {
            var real = await _applicationClient.GetApplicationByIdAsync(applicationId);
            if (real == null)
            {
                if (cached != null)
                    await appCacheRepo.DeleteAsync(applicationId);

                return null;
            }

            var updated = _mapper.Map<CachedApplication>(real);
            updated.UpdatedAt = DateTime.UtcNow;

            await appCacheRepo.UpdateAsync(updated);
            return updated;
        }

        return cached;
    }
}

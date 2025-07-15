using AutoMapper;
using CandidateService.BLL.Clients;
using CandidateService.BLL.Enum;
using CandidateService.BLL.Models;
using CandidateService.BLL.Services.Interfaces;
using CandidateService.BLL.Validations;
using CandidateService.DAL.UoF;
using DAL_Core.Entities;
using DAL_Core.Enum;
using InterviewingSystem.Contracts.IntegrationEvents;
using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace CandidateService.BLL.Services;
public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IPositionClient _positionClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private const int PositionCacheDays = 3; // Cache positions for 3 days

    public ApplicationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IBlobStorageService blobStorageService,
        IPositionClient positionClient,
        IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
        _positionClient = positionClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ApplicationDto> GetApplicationByIdAsync(Guid id)
    {
        var appRepository = _unitOfWork.ApplicationRepository;

        var application = await appRepository.GetWithCandidateProfileAsync(id);

        if (application == null)
            throw new NotFoundException("Application", id);

        var applicationDto = _mapper.Map<ApplicationDto>(application);

        await GetResumeUriAsync(applicationDto);

        var position = await CheckAndRefreshPositionAsync(application.PositionId);
        applicationDto.PositionTitle = position?.Title ?? "Position unavailable";

        return applicationDto;
    }

    public async Task<IList<ApplicationDto>> GetAllApplicationsAsync()
    {
        var appRepository = _unitOfWork.ApplicationRepository;
        var cachedPositionRepository = _unitOfWork.CachedPositionRepository;

        var applications = await appRepository.GetAllWithCandidateProfileAsync();

        var applicationDtos = _mapper.Map<IList<ApplicationDto>>(applications);

        foreach (var dto in applicationDtos)
        {
            await GetResumeUriAsync(dto);

            var position = await CheckAndRefreshPositionAsync(dto.PositionId);
            dto.PositionTitle = position?.Title ?? "Position unavailable";
        }

        return applicationDtos;
    }

    public async Task<Guid> CreateApplicationAsync(CreateApplicationDto model)
    {
        var appRepository = _unitOfWork.ApplicationRepository;
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;

        var existingApplication = await appRepository.GetAsync(model.Id);
        if (existingApplication != null)
            throw new ArgumentException($"Application with ID={model.Id} already exists");

        var existingCandidateProfile = await candidateProfileRepository.ExistsAsync(model.CandidateProfileId);
        if (!existingCandidateProfile)
            throw new NotFoundException("CandidateProfile", model.CandidateProfileId);


        var application = new Application
        {
            Id = Guid.NewGuid(),
            CandidateProfileId = model.CandidateProfileId,
            Status = ApplicationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var position = await CheckAndRefreshPositionAsync(model.PositionId);
        if (position == null)
            throw new ValidationAppException($"Position with ID={model.PositionId} not found or inactive");
        application.PositionId = model.PositionId;

        if (model.ResumeFile != null)
        {
            using var stream = model.ResumeFile.OpenReadStream();
            var extension = Path.GetExtension(model.ResumeFile.FileName);
            var blobPath = $"applications/{application.Id}{extension}";

            var uploadedPath = await _blobStorageService.UploadOrReplaceResumeAsync(stream, model.ResumeFile.FileName, blobPath);
            application.ResumeBlobPath = uploadedPath;
        }
        else
        {
            // fallback: try to use profile resume
            var profileResumePath = await candidateProfileRepository.GetProfileResumePathAsync(model.CandidateProfileId);
            if (string.IsNullOrEmpty(profileResumePath))
                throw new InvalidOperationAppException("No resume provided and candidate profile has no resume.");
            application.ResumeBlobPath = profileResumePath;
        }
        var id = await appRepository.CreateAsync(application);
        await _unitOfWork.SaveChangesAsync();
        return id;
    }

    public async Task<IList<ApplicationDto>> GetApplicationsByPositionIdAsync(Guid positionId)
    {
        var appRepository = _unitOfWork.ApplicationRepository;
        var cachedPositionRepository = _unitOfWork.CachedPositionRepository;

        var position = await CheckAndRefreshPositionAsync(positionId);
        if (position == null)
            throw new ValidationException($"Position with ID={positionId} not found or inactive");

        var applications = await appRepository.GetApplicationsByPositionId(positionId);
        var applicationDtos = _mapper.Map<IList<ApplicationDto>>(applications);
        foreach (var dto in applicationDtos)
        {
            await GetResumeUriAsync(dto);
            dto.PositionTitle = position.Title;
        }
        return applicationDtos;
    }

    public async Task<Guid> UpdateApplicationAsync(CreateApplicationDto model)
    {
        var appRepository = _unitOfWork.ApplicationRepository;
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;

        var application = await appRepository.GetAsync(model.Id);
        if (application == null)
            throw new NotFoundException("Application", model.Id);

        if (application.Status == ApplicationStatus.InterviewScheduled)
            throw new InvalidOperationAppException("Cannot update application after interview is scheduled.");

        _mapper.Map(model, application);

        var position = await CheckAndRefreshPositionAsync(model.PositionId);
        if (position == null)
            throw new ValidationException($"Position with ID={model.PositionId} not found or inactive");
        application.PositionId = model.PositionId;

        if (model.ResumeFile != null)
        {
            if (!string.IsNullOrEmpty(application.ResumeBlobPath))
                await _blobStorageService.DeleteFileAsync(application.ResumeBlobPath);

            using var stream = model.ResumeFile.OpenReadStream();
            var extension = Path.GetExtension(model.ResumeFile.FileName);
            var blobPath = $"applications/{application.Id}{extension}";

            var uploadedPath = await _blobStorageService.UploadOrReplaceResumeAsync(stream, model.ResumeFile.FileName, blobPath);
            application.ResumeBlobPath = uploadedPath;
        }
        else if (string.IsNullOrEmpty(application.ResumeBlobPath))
        {
            // fallback: use profile resume
            var candidateProfile = await candidateProfileRepository.GetAsync(application.CandidateProfileId);
            if (candidateProfile == null)
                throw new NotFoundException("Application", application.CandidateProfileId);

            if (string.IsNullOrEmpty(candidateProfile.ProfileResumeBlobPath))
                throw new InvalidOperationAppException("No resume provided and candidate profile has no resume.");

            application.ResumeBlobPath = candidateProfile.ProfileResumeBlobPath;
        }

        await appRepository.UpdateAsync(application);
        await _unitOfWork.SaveChangesAsync();

        return application.Id;
    }

    public async Task DeleteApplicationAsync(Guid id)
    {
        var appRepository = _unitOfWork.ApplicationRepository;

        var application = await appRepository.GetAsync(id);

        if (application == null)
            throw new NotFoundException("Application", id);

        if (application.Status == ApplicationStatus.InterviewScheduled)
            throw new("Cannot delete application after interview is scheduled.");

        if (!string.IsNullOrEmpty(application.ResumeBlobPath))
        {
            // Check if the resume belongs to the candidate profile
            var profilePrefix = $"profiles/{application.CandidateProfileId}";
            if (!application.ResumeBlobPath.StartsWith(profilePrefix, StringComparison.OrdinalIgnoreCase))
            {
                await _blobStorageService.DeleteFileAsync(application.ResumeBlobPath);
            }
        }

        await appRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsApplicationOwnedByUserAsync(Guid applicationId, Guid userId)
    {
        var appRepository = _unitOfWork.ApplicationRepository;

        var application = await appRepository.GetWithCandidateProfileAsync(applicationId);

        return application != null && application.CandidateProfile.UserId == userId;
    }

    public async Task ApproveApplicationAsync(Guid applicationId)
    {
        var appRepository = _unitOfWork.ApplicationRepository;
        var application = await appRepository.GetWithCandidateProfileAsync(applicationId);

        if (application == null)
            throw new NotFoundException("Application", applicationId);

        if (application.Status == ApplicationStatus.InterviewScheduled)
            throw new InvalidOperationAppException("Interview is already scheduled for this application.");

        application.Status = ApplicationStatus.Approved;
        await appRepository.UpdateAsync(application);
        await _unitOfWork.SaveChangesAsync();

        var evt = _mapper.Map<ApplicationApprovedEvent>(application);
        evt.AppliedAt = DateTime.UtcNow;

        await _publishEndpoint.Publish(evt);
    }

    public async Task ScheduleInterviewAsync(Guid applicationId, Guid interviewId)
    {
        var appRepository = _unitOfWork.ApplicationRepository;
        var application = await appRepository.GetAsync(applicationId);

        if (application == null)
            throw new NotFoundException("Application", applicationId);

        if (application.Status == ApplicationStatus.InterviewScheduled)
            throw new InvalidOperationAppException("Interview is already scheduled for this application.");

        if (application.Status == ApplicationStatus.Rejected || application.Status == ApplicationStatus.Hired)
            throw new InvalidOperationAppException("Cannot schedule interview for finalized application.");

        application.InterviewId = interviewId;
        application.Status = ApplicationStatus.InterviewScheduled;

        await appRepository.UpdateAsync(application);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task FinalizeApplicationAsync(Guid applicationId, ApplicationFinalStatus finalStatus)
    {
        var appRepository = _unitOfWork.ApplicationRepository;
        var application = await appRepository.GetAsync(applicationId);

        if (application == null)
            throw new NotFoundException("Application", applicationId);

        if (application.Status != ApplicationStatus.InterviewScheduled)
            throw new InvalidOperationAppException("Application already finalized.");

        application.Status = finalStatus switch
        {
            ApplicationFinalStatus.Hired => ApplicationStatus.Hired,
            ApplicationFinalStatus.Rejected => ApplicationStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(finalStatus), "Unsupported final status")
        };

        await appRepository.UpdateAsync(application);
        await _unitOfWork.SaveChangesAsync();

        var evt = new ApplicationFinalizedEvent
        {
            Id = applicationId
        };

        await _publishEndpoint.Publish(evt);
    }

    private async Task<CachedPosition?> CheckAndRefreshPositionAsync(Guid positionId)
    {
        var cachedPositionRepository = _unitOfWork.CachedPositionRepository;
        var cached = await cachedPositionRepository.GetAsync(positionId);

        // Check if the cached position is stale (older than 3 days) or not active
        bool isStale = cached == null || cached.UpdatedAt < DateTime.UtcNow.AddDays(-PositionCacheDays);

        if (cached == null || !cached.IsActive || isStale)
        {
            var real = await _positionClient.GetPositionByIdAsync(positionId);

            if (real == null || !real.IsActive)
            {
                await cachedPositionRepository.DeleteAsync(positionId);
                return null;
            }

            var updated = _mapper.Map<CachedPosition>(real);
            updated.UpdatedAt = DateTime.UtcNow;

            await cachedPositionRepository.UpsertCacheAsync(updated);
            return updated;
        }

        return cached;
    }

    private async Task GetResumeUriAsync(ApplicationDto dto)
    {
        if (!string.IsNullOrEmpty(dto.ResumeUrl))
            // Assuming dto.ProfileResumeUrl contains the blob file path name
            dto.ResumeUrl = await _blobStorageService.GetResumeSasUriAsync(dto.ResumeUrl);
    }
}


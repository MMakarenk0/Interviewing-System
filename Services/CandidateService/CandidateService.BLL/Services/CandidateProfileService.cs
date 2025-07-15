using AutoMapper;
using CandidateService.BLL.Models;
using CandidateService.BLL.Services.Interfaces;
using CandidateService.DAL.UoF;
using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CandidateService.BLL.Services;
public class CandidateProfileService : ICandidateProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;

    public CandidateProfileService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IBlobStorageService blobStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }

    public async Task<CandidateProfileDto> GetCandidateProfileByIdAsync(Guid id)
    {
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;

        var candidateProfile = await candidateProfileRepository.GetAsync(id);
        if (candidateProfile == null)
            throw new ArgumentException($"Candidate profile with ID={id} does not exist");

        var candidateProfileDto = _mapper.Map<CandidateProfileDto>(candidateProfile);

        await GetResumeUriAsync(candidateProfileDto);

        return candidateProfileDto;
    }

    public async Task<IList<CandidateProfileDto>> GetAllCandidateProfilesAsync()
    {
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;
        var candidateProfiles = await candidateProfileRepository.GetAllAsync();

        var candidateProfileDtos = _mapper.Map<IList<CandidateProfileDto>>(candidateProfiles);

        foreach (var dto in candidateProfileDtos)
        {
            await GetResumeUriAsync(dto);
        }

        return candidateProfileDtos;
    }

    public async Task<IList<CandidateProfileDto>> GetCandidateProfilesByUserIds(IEnumerable<Guid> userIds)
    {
        if (userIds == null || !userIds.Any())
            return new List<CandidateProfileDto>();

        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;
        var candidateProfiles = await candidateProfileRepository.GetCandidateProfilesByUserIds(userIds);

        if (candidateProfiles == null || !candidateProfiles.Any())
            return new List<CandidateProfileDto>();

        var candidateProfileDtos = _mapper.Map<IList<CandidateProfileDto>>(candidateProfiles);
        foreach (var dto in candidateProfileDtos)
        {
            await GetResumeUriAsync(dto);
        }
        return candidateProfileDtos;
    }

    public async Task<IList<CandidateProfileDto>> FilterCandidateProfilesAsync(CandidateProfileQuery query)
    {
        var profilesQuery = _unitOfWork.CandidateProfileRepository.Query();

        // Filter by user IDs
        if (query.UserIds?.Any() == true)
        {
            profilesQuery = profilesQuery.Where(p => query.UserIds.Contains(p.UserId));
        }

        // Soft search: position (case-insensitive, partial)
        if (!string.IsNullOrWhiteSpace(query.Position))
        {
            var position = query.Position.Trim().ToLowerInvariant();
            profilesQuery = profilesQuery.Where(p =>
                !string.IsNullOrEmpty(p.CurrentPosition) &&
                p.CurrentPosition.ToLower().Contains(position));
        }

        // Filter by YearsOfExperience
        if (!string.IsNullOrWhiteSpace(query.YearsOfExperienceOperator) && query.YearsOfExperienceValue.HasValue)
        {
            var op = query.YearsOfExperienceOperator;
            var val = query.YearsOfExperienceValue.Value;

            profilesQuery = op switch
            {
                ">" => profilesQuery.Where(p => p.YearsOfExperience > val),
                ">=" => profilesQuery.Where(p => p.YearsOfExperience >= val),
                "<" => profilesQuery.Where(p => p.YearsOfExperience < val),
                "<=" => profilesQuery.Where(p => p.YearsOfExperience <= val),
                "=" or _ => profilesQuery.Where(p => p.YearsOfExperience == val),
            };
        }

        var candidateProfiles = await profilesQuery.ToListAsync();

        if (!string.IsNullOrWhiteSpace(query.TechStack))
        {
            var tech = NormalizeString(query.TechStack);
            candidateProfiles = candidateProfiles
                .Where(p => !string.IsNullOrEmpty(p.TechStack) &&
                            NormalizeString(p.TechStack).Contains(tech))
                .ToList();
        }

        var candidateProfileDtos = _mapper.Map<IList<CandidateProfileDto>>(candidateProfiles);

        foreach (var dto in candidateProfileDtos)
        {
            await GetResumeUriAsync(dto);
        }

        return candidateProfileDtos;
    }


    public async Task<Guid> CreateCandidateProfileAsync(CreateCandidateProfileDto candidateProfileModel)
    {
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;

        var candidateProfile = await candidateProfileRepository.GetAsync(candidateProfileModel.Id);
        if (candidateProfile != null)
            throw new ArgumentException($"Candidate profile with ID={candidateProfileModel.Id} already exists");

        candidateProfile = _mapper.Map<CandidateProfile>(candidateProfileModel);

        if (candidateProfileModel.ProfileResumeFile != null)
        {
            var file = candidateProfileModel.ProfileResumeFile;
            var fileName = file.FileName;
            var stream = file.OpenReadStream();

            var blobPathPrefix = $"candidates/{candidateProfile.Id}";
            var uploadedBlobPath = await _blobStorageService.UploadOrReplaceResumeAsync(stream, fileName, blobPathPrefix);

            candidateProfile.ProfileResumeBlobPath = uploadedBlobPath;
        }

        if (candidateProfileModel.ApplicationIds.Any())
        {
            var _applicationRepository = _unitOfWork.ApplicationRepository;

            candidateProfile.Applications = new List<Application>();

            foreach (var applicationId in candidateProfileModel.ApplicationIds)
            {
                var application = await _applicationRepository.GetAsync(applicationId);
                if (application == null)
                    throw new ArgumentException($"Application with ID={applicationId} not found");

                application.CandidateProfileId = candidateProfile.Id;
                application.CandidateProfile = candidateProfile;

                candidateProfile.Applications.Add(application);
            }
        }

        await candidateProfileRepository.CreateAsync(candidateProfile);
        await _unitOfWork.SaveChangesAsync();

        return candidateProfile.Id;
    }

    public async Task DeleteCandidateProfileAsync(Guid id)
    {
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;

        var profile = await candidateProfileRepository.GetAsync(id);
        if (profile == null)
            throw new ArgumentException($"Candidate profile with ID={id} not found");

        if (!string.IsNullOrEmpty(profile.ProfileResumeBlobPath))
            await _blobStorageService.DeleteFileAsync(profile.ProfileResumeBlobPath);

        await candidateProfileRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Guid> UpdateCandidateProfileAsync(CreateCandidateProfileDto candidateProfileModel)
    {
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;
        var applicationRepository = _unitOfWork.ApplicationRepository;

        var candidateProfile = await candidateProfileRepository.GetCandidateProfileWithApplicationsAsync(candidateProfileModel.Id);
        if (candidateProfile == null)
            throw new ArgumentException($"Candidate profile with ID={candidateProfileModel.Id} not found");

        _mapper.Map(candidateProfileModel, candidateProfile);

        if (candidateProfileModel.ProfileResumeFile != null)
        {
            if (!string.IsNullOrEmpty(candidateProfile.ProfileResumeBlobPath))
            {
                await _blobStorageService.DeleteFileAsync(candidateProfile.ProfileResumeBlobPath);
            }

            using var stream = candidateProfileModel.ProfileResumeFile.OpenReadStream();
            var originalFileName = candidateProfileModel.ProfileResumeFile.FileName;
            var extension = Path.GetExtension(originalFileName);

            var blobPath = $"profiles/{candidateProfile.Id}{extension}";

            var uploadedPath = await _blobStorageService.UploadOrReplaceResumeAsync(stream, originalFileName, blobPath);

            candidateProfile.ProfileResumeBlobPath = uploadedPath;
        }

        if (candidateProfileModel.ApplicationIds.Any())
        {
            var currentAppIds = candidateProfile.Applications.Select(a => a.Id).ToList();

            var removedApps = candidateProfile.Applications
                .Where(a => !candidateProfileModel.ApplicationIds.Contains(a.Id))
                .ToList();

            foreach (var removed in removedApps)
            {
                candidateProfile.Applications.Remove(removed);
            }

            var newAppIds = candidateProfileModel.ApplicationIds
                .Where(id => !currentAppIds.Contains(id))
                .ToList();

            foreach (var appId in newAppIds)
            {
                var application = await applicationRepository.GetAsync(appId);
                if (application == null)
                    throw new ArgumentException($"Application with ID={appId} not found");

                application.CandidateProfileId = candidateProfile.Id;
                candidateProfile.Applications.Add(application);
            }
        }

        await candidateProfileRepository.UpdateAsync(candidateProfile);
        await _unitOfWork.SaveChangesAsync();

        return candidateProfile.Id;
    }

    public async Task<bool> IsProfileOwnedByUserAsync(Guid profileId, Guid userId)
    {
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;

        var profile = await candidateProfileRepository.GetAsync(profileId);

        return profile != null && profile.UserId == userId;
    }

    public async Task<CandidateProfileDto?> GetCandidateProfileByUserIdAsync(Guid userId)
    {
        var candidateProfileRepository = _unitOfWork.CandidateProfileRepository;
        var profile = await candidateProfileRepository.GetCandidateProfileByUserIdAsync(userId);
        if (profile == null)
            return null;
        var profileDto = _mapper.Map<CandidateProfileDto>(profile);
        if (!string.IsNullOrEmpty(profile.ProfileResumeBlobPath))
            profileDto.ProfileResumeUrl = await _blobStorageService.GetResumeSasUriAsync(profile.ProfileResumeBlobPath);
        return profileDto;
    }

    private async Task GetResumeUriAsync(CandidateProfileDto dto)
    {
        if (!string.IsNullOrEmpty(dto.ProfileResumeUrl))
            // Assuming dto.ProfileResumeUrl contains the blob file path name
            dto.ProfileResumeUrl = await _blobStorageService.GetResumeSasUriAsync(dto.ProfileResumeUrl);
    }

    private static string NormalizeString(string input)
    {
        return new string(input
            .Where(char.IsLetterOrDigit)
            .ToArray())
            .ToLowerInvariant();
    }
}


namespace CandidateService.BLL.Profile;

using AutoMapper;
using CandidateService.BLL.Models;
using DAL_Core.Entities;
using InterviewingSystem.Contracts.IntegrationEvents;

public class CandidateManagmentMappingProfile : Profile
{
    public CandidateManagmentMappingProfile()
    {
        CreateMap<CreateCandidateProfileDto, CandidateProfile>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))
            .ForMember(dest => dest.TechStack, opt => opt.MapFrom(src => src.TechStack))
            .ForMember(dest => dest.CurrentPosition, opt => opt.MapFrom(src => src.CurrentPosition));

        CreateMap<CandidateProfile, CandidateProfileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Applications, opt => opt.MapFrom(src => src.Applications))
            .ForMember(dest => dest.ProfileResumeUrl, opt => opt.MapFrom(src => src.ProfileResumeBlobPath)); // temporary data for SAS link generation with O(n)

        CreateMap<Application, ApplicationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CandidateProfileId, opt => opt.MapFrom(src => src.CandidateProfileId))
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.PositionId))
            .ForMember(dest => dest.PositionTitle, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ResumeUrl, opt => opt.MapFrom(src => src.ResumeBlobPath)); // temporary data for SAS link generation with O(n)

        CreateMap<CreateApplicationDto, Application>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore())
            .ForMember(dest => dest.PositionId, opt => opt.Ignore())
            .ForMember(dest => dest.ResumeBlobPath, opt => opt.Ignore())
            .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore());


        CreateMap<CachedPosition, PositionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ReverseMap();

        CreateMap<Application, ApplicationApprovedEvent>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CandidateProfileId, opt => opt.MapFrom(src => src.CandidateProfileId))
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.PositionId));
    }
}


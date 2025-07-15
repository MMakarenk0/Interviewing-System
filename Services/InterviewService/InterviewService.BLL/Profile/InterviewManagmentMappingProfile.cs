namespace InterviewService.BLL.Profile;
using AutoMapper;
using DAL_Core.Entities;
using InterviewingSystem.Contracts.IntegrationEvents;
using InterviewService.BLL.Models;

public class InterviewManagmentMappingProfile : Profile
{
    public InterviewManagmentMappingProfile()
    {
        CreateMap<ApplicationDto, CachedApplication>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CandidateProfileId, opt => opt.MapFrom(src => src.CandidateProfileId))
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.PositionId));

        CreateMap<ApplicationApprovedEvent, CachedApplication>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CandidateProfileId, opt => opt.MapFrom(src => src.CandidateProfileId))
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.PositionId));

        CreateMap<Position, PositionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<CreatePositionDto, Position>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<Assessment, AssessmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TechnicalScore, opt => opt.MapFrom(src => src.TechnicalScore))
            .ForMember(dest => dest.CommunicationScore, opt => opt.MapFrom(src => src.CommunicationScore))
            .ForMember(dest => dest.Feedback, opt => opt.MapFrom(src => src.Feedback))
            .ForMember(dest => dest.InterviewId, opt => opt.MapFrom(src => src.InterviewId))
            .ForMember(dest => dest.InterviewerFullName, opt => opt.MapFrom(src => src.InterviewId));

        CreateMap<CreateAssessmentDto, Assessment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TechnicalScore, opt => opt.MapFrom(src => src.TechnicalScore))
            .ForMember(dest => dest.CommunicationScore, opt => opt.MapFrom(src => src.CommunicationScore))
            .ForMember(dest => dest.Feedback, opt => opt.MapFrom(src => src.Feedback))
            .ForMember(dest => dest.InterviewId, opt => opt.MapFrom(src => src.InterviewId));
    }
}
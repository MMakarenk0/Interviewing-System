using AutoMapper;
using DAL_Core.Entities;
using InterviewService.BLL.Models;
using InterviewService.BLL.Services.Interfaces;
using InterviewService.DAL.UoF;

namespace InterviewService.BLL.Services;

public class AssessmentService : IAssessmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AssessmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AssessmentDto>> GetAllAssessmentsAsync()
    {
        var assessments = await _unitOfWork.AssessmentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<AssessmentDto>>(assessments);
    }

    public async Task<AssessmentDto> GetAssessmentByIdAsync(Guid id)
    {
        var assessmentRepository = _unitOfWork.AssessmentRepository;
        var assessment = await assessmentRepository.GetAsync(id);
        if (assessment == null)
            throw new ArgumentException($"Assessment with ID={id} not found");
        return _mapper.Map<AssessmentDto>(assessment);
    }

    public async Task<Guid> CreateAssessmentAsync(CreateAssessmentDto assessmentDto)
    {
        var assessmentRepository = _unitOfWork.AssessmentRepository;
        var interviewRepository = _unitOfWork.InterviewRepository;

        var interview = await interviewRepository.GetWithSlotAsync(assessmentDto.InterviewId);
        if (interview == null)
            throw new ArgumentException($"Interview with ID={assessmentDto.InterviewId} not found");

        var existing = await assessmentRepository.GetAsync(assessmentDto.Id);
        if (existing != null)
            throw new InvalidOperationException($"Assessment with ID={assessmentDto.Id} already exists.");

        if (assessmentDto.InterviewerId != Guid.Empty &&
            assessmentDto.InterviewerId != interview.Slot.InterviewerId)
        {
            throw new InvalidOperationException("Specified interviewer does not match the interview slot.");
        }

        var assessment = _mapper.Map<Assessment>(assessmentDto);
        await assessmentRepository.CreateAsync(assessment);
        await _unitOfWork.SaveChangesAsync();

        return assessment.Id;
    }
}


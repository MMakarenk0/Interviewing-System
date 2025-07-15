using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL_Core.Entities;
using InterviewService.BLL.Models;
using InterviewService.BLL.Services.Interfaces;
using InterviewService.DAL.UoF;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.BLL.Services;

public class PositionService : IPositionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PositionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PositionDto>> GetAllPositionsAsync()
    {
        var positions = await _unitOfWork.PositionRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PositionDto>>(positions);
    }

    public async Task<PositionDto> GetPositionByIdAsync(Guid id)
    {
        var positionRepository = _unitOfWork.PositionRepository;

        var position = await positionRepository.GetAsync(id);
        return _mapper.Map<PositionDto>(position);
    }

    public async Task<PagedResult<PositionDto>> GetPagedPositionsAsync(PositionQueryParameters parameters)
    {
        var positionRepository = _unitOfWork.PositionRepository;
        var query = positionRepository.GetAll();

        if (!string.IsNullOrEmpty(parameters.TitleFilter))
            query = query.Where(p => p.Title.Contains(parameters.TitleFilter));

        if (parameters.IsActive.HasValue)
            query = query.Where(p => p.IsActive == parameters.IsActive.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ProjectTo<PositionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<PositionDto>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<Guid> CreatePositionAsync(CreatePositionDto positionDto)
    {
        var positionRepository = _unitOfWork.PositionRepository;
        var position = await positionRepository.GetAsync(positionDto.Id);
        if (position != null)
            throw new InvalidOperationException($"Position with the ID={positionDto.Id} already exists.");

        position = _mapper.Map<Position>(positionDto);
        position.CreatedAt = DateTime.UtcNow;

        var positionId = await positionRepository.CreateAsync(position);
        await _unitOfWork.SaveChangesAsync();
        return positionId;
    }

    public async Task<PositionDto> UpdatePositionAsync(Guid id, PositionDto positionDto)
    {
        var position = await _unitOfWork.PositionRepository.GetAsync(id);
        if (position == null)
        {
            return null;
        }
        _mapper.Map(positionDto, position);
        await _unitOfWork.PositionRepository.UpdateAsync(position);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PositionDto>(position);
    }
}


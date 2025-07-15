using DAL_Core.Entities;

namespace InterviewService.DAL.Repositories.Interfaces;
public interface IPositionRepository : IRepository<Position>
{
    IQueryable<Position> GetAll();
}
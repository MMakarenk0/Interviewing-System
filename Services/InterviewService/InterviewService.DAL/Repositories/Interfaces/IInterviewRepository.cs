using DAL_Core.Entities;

namespace InterviewService.DAL.Repositories.Interfaces;
public interface IInterviewRepository : IRepository<Interview>
{
    Task<ICollection<Interview>> GetAllWithSlotAsync();
    Task<Interview?> GetWithSlotAsync(Guid id);
}
using DAL_Core;
using DAL_Core.Entities;
using InterviewService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.DAL.Repositories;
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly InterviewingSystemDbContext _ctx;

    public Repository(InterviewingSystemDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ICollection<T>> GetAllAsync()
    {
        return await _ctx.Set<T>().ToListAsync();
    }

    public async Task<T?> GetAsync(Guid id)
    {
        return await _ctx.Set<T>().FindAsync(id);
    }

    public async Task<Guid> CreateAsync(T entity)
    {
        await _ctx.Set<T>().AddAsync(entity);

        return entity.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _ctx.Set<T>().FindAsync(id);

        if (entity == null)
        {
            return;
        }

        _ctx.Set<T>().Remove(entity);
    }

    public async Task<Guid> UpdateAsync(T entity)
    {
        _ctx.Set<T>().Update(entity);

        return entity.Id;
    }
}
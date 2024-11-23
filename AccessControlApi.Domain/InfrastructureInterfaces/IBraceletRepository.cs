using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.InfrastructureInterfaces;

public interface IBraceletRepository
{
    Task<Bracelet> GetByIdAsync(int id);
    Task<IEnumerable<Bracelet>> GetAllAsync();
    Task<int> CreateAsync(Bracelet bracelet);
    Task UpdateAsync(Bracelet bracelet);
    Task DeleteAsync(int id);
}

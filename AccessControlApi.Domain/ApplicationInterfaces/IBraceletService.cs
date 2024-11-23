using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.ApplicationInterfaces;

public interface IBraceletService
{
    Task<Bracelet> GetByIdAsync(int id);
    Task<IEnumerable<Bracelet>> GetAllAsync();
    Task<int> CreateAsync(Bracelet bracelet);
    Task UpdateAsync(Bracelet bracelet);
    Task DeleteAsync(int id);
}

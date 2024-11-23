using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.InfrastructureInterfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> GetByIdAsync(int id);
    Task<int> CreateAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(int id);
    Task<Role> GetRoleByIdAsync(int roleId);
}

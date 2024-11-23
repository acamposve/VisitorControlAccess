using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.ApplicationInterfaces;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<Role> GetRoleByIdAsync(int id);
    Task<int> CreateRoleAsync(Role role);
    Task UpdateRoleAsync(Role role);
    Task DeleteRoleAsync(int id);
}

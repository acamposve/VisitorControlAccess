using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.InfrastructureInterfaces;

public interface IAccessPointRepository
{
    Task<IEnumerable<AccessPoint>> GetAllAccessPointsAsync();
    Task<AccessPoint> GetAccessPointByIdAsync(int id);
    Task<int> CreateAccessPointAsync(AccessPoint accessPoint);
    Task<bool> UpdateAccessPointAsync(AccessPoint accessPoint);
    Task<bool> DeleteAccessPointAsync(int id);
}

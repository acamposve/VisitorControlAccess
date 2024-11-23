using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.ApplicationInterfaces;

public interface IAccessPointService
{
    Task<IEnumerable<AccessPoint>> GetAllAccessPointsAsync();
    Task<AccessPoint> GetAccessPointByIdAsync(int id);
    Task<int> CreateAccessPointAsync(AccessPoint accessPoint);
    Task<bool> UpdateAccessPointAsync(AccessPoint accessPoint);
    Task<bool> DeleteAccessPointAsync(int id);
}

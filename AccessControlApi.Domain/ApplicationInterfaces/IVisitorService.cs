using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.ApplicationInterfaces;

public interface IVisitorService
{
    Task<IEnumerable<Visitor>> GetAllVisitorsAsync();
    Task<Visitor> GetVisitorByIdAsync(int id);
    Task<int> CreateVisitorAsync(Visitor visitor);
    Task<bool> UpdateVisitorAsync(Visitor visitor);
    Task<bool> DeleteVisitorAsync(int id);
}

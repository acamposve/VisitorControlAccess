using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;

namespace AccessControlApi.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly IVisitorRepository _visitorRepository;

    public VisitorService(IVisitorRepository visitorRepository)
    {
        _visitorRepository = visitorRepository;
    }

    public async Task<IEnumerable<Visitor>> GetAllVisitorsAsync()
    {
        return await _visitorRepository.GetAllVisitorsAsync();
    }

    public async Task<Visitor> GetVisitorByIdAsync(int id)
    {
        return await _visitorRepository.GetVisitorByIdAsync(id);
    }

    public async Task<int> CreateVisitorAsync(Visitor visitor)
    {
        return await _visitorRepository.CreateVisitorAsync(visitor);
    }

    public async Task<bool> UpdateVisitorAsync(Visitor visitor)
    {
        return await _visitorRepository.UpdateVisitorAsync(visitor);
    }

    public async Task<bool> DeleteVisitorAsync(int id)
    {
        return await _visitorRepository.DeleteVisitorAsync(id);
    }
}

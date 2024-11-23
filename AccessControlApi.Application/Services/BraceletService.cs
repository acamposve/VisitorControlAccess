using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;

namespace AccessControlApi.Application.Services;

public class BraceletService : IBraceletService
{
    private readonly IBraceletRepository _braceletRepository;

    public BraceletService(IBraceletRepository braceletRepository)
    {
        _braceletRepository = braceletRepository;
    }

    public async Task<Bracelet> GetByIdAsync(int id)
    {
        return await _braceletRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Bracelet>> GetAllAsync()
    {
        return await _braceletRepository.GetAllAsync();
    }

    public async Task<int> CreateAsync(Bracelet bracelet)
    {
        return await _braceletRepository.CreateAsync(bracelet);
    }

    public async Task UpdateAsync(Bracelet bracelet)
    {
        await _braceletRepository.UpdateAsync(bracelet);
    }

    public async Task DeleteAsync(int id)
    {
        await _braceletRepository.DeleteAsync(id);
    }
}

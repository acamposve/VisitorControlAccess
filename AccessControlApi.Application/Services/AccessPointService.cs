using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using AccessControlApi.Domain.Exceptions;

namespace AccessControlApi.Application.Services;

public class AccessPointService : IAccessPointService
{
    private readonly IAccessPointRepository _accessPointRepository;

    public AccessPointService(IAccessPointRepository accessPointRepository)
    {
        _accessPointRepository = accessPointRepository ??
            throw new ArgumentNullException(nameof(accessPointRepository));
    }

    public async Task<IEnumerable<AccessPoint>> GetAllAccessPointsAsync()
    {
        try
        {
            return await _accessPointRepository.GetAllAccessPointsAsync();
        }
        catch (Exception ex)
        {
            throw new AccessControlException("Error retrieving access points", ex);
        }
    }

    public async Task<AccessPoint> GetAccessPointByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than zero", nameof(id));

        try
        {
            var accessPoint = await _accessPointRepository.GetAccessPointByIdAsync(id);
            if (accessPoint == null)
                throw new NotFoundException($"Access point with ID {id} not found");

            return accessPoint;
        }
        catch (Exception ex) when (!(ex is ArgumentException || ex is NotFoundException))
        {
            throw new AccessControlException($"Error retrieving access point with ID {id}", ex);
        }
    }

    public async Task<int> CreateAccessPointAsync(AccessPoint accessPoint)
    {
        if (accessPoint == null)
            throw new ArgumentNullException(nameof(accessPoint));

        ValidateAccessPoint(accessPoint);

        try
        {
            return await _accessPointRepository.CreateAccessPointAsync(accessPoint);
        }
        catch (Exception ex)
        {
            throw new AccessControlException("Error creating access point", ex);
        }
    }

    public async Task<bool> UpdateAccessPointAsync(AccessPoint accessPoint)
    {
        if (accessPoint == null)
            throw new ArgumentNullException(nameof(accessPoint));

        if (accessPoint.Id <= 0)
            throw new ArgumentException("Access point Id must be greater than zero", nameof(accessPoint));

        ValidateAccessPoint(accessPoint);

        try
        {
            var updated = await _accessPointRepository.UpdateAccessPointAsync(accessPoint);
            if (!updated)
                throw new NotFoundException($"Access point with ID {accessPoint.Id} not found");

            return true;
        }
        catch (Exception ex) when (!(ex is ArgumentException || ex is NotFoundException))
        {
            throw new AccessControlException($"Error updating access point with ID {accessPoint.Id}", ex);
        }
    }

    public async Task<bool> DeleteAccessPointAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than zero", nameof(id));

        try
        {
            var deleted = await _accessPointRepository.DeleteAccessPointAsync(id);
            if (!deleted)
                throw new NotFoundException($"Access point with ID {id} not found");

            return true;
        }
        catch (Exception ex) when (!(ex is ArgumentException || ex is NotFoundException))
        {
            throw new AccessControlException($"Error deleting access point with ID {id}", ex);
        }
    }

    private void ValidateAccessPoint(AccessPoint accessPoint)
    {
        if (string.IsNullOrWhiteSpace(accessPoint.Name))
            throw new ArgumentException("Name is required", nameof(accessPoint));

        if (accessPoint.CreatedBy <= 0)
            throw new ArgumentException("Created by user ID must be greater than zero", nameof(accessPoint));
    }
}
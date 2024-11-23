using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using AccessControlApi.Infrastructure.Wrappers;
using System.Data;

public class AccessPointRepository : IAccessPointRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDapperWrapper _dapper;

    public AccessPointRepository(IDbConnection dbConnection, IDapperWrapper dapper)
    {
        _dbConnection = dbConnection;
        _dapper = dapper;
    }

    public async Task<IEnumerable<AccessPoint>> GetAllAccessPointsAsync()
    {
        const string query = "SELECT * FROM AccessPoints";
        return await _dapper.QueryAsync<IEnumerable<AccessPoint>>(_dbConnection, query);
    }

    public async Task<AccessPoint> GetAccessPointByIdAsync(int id)
    {
        const string query = "SELECT * FROM AccessPoints WHERE Id = @Id";
        return await _dapper.QuerySingleOrDefaultAsync<AccessPoint>(_dbConnection, query, new { Id = id });
    }

    public async Task<int> CreateAccessPointAsync(AccessPoint accessPoint)
    {
        const string query = @"
                INSERT INTO AccessPoints (Name, Description, CreatedBy, CreatedAt)
                VALUES (@Name, @Description, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _dapper.QuerySingleAsync<int>(_dbConnection, query, accessPoint);
    }

    public async Task<bool> UpdateAccessPointAsync(AccessPoint accessPoint)
    {
        const string query = @"
                UPDATE AccessPoints
                SET Name = @Name, 
                    Description = @Description, 
                    CreatedBy = @CreatedBy, 
                    CreatedAt = @CreatedAt
                WHERE Id = @Id";
        var result = await _dapper.ExecuteAsync(_dbConnection, query, accessPoint);
        return result > 0;
    }

    public async Task<bool> DeleteAccessPointAsync(int id)
    {
        const string query = "DELETE FROM AccessPoints WHERE Id = @Id";
        var result = await _dapper.ExecuteAsync(_dbConnection, query, new { Id = id });
        return result > 0;
    }
}
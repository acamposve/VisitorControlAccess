using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using AccessControlApi.Infrastructure.Wrappers;
using Dapper;
using System.Data;

namespace AccessControlApi.Infrastructure.Repositories;

public class VisitorRepository : IVisitorRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDapperWrapper _dapper;


    public VisitorRepository(IDbConnection dbConnection, IDapperWrapper dapper)
    {
        _dbConnection = dbConnection;
        _dapper = dapper;
    }


    public async Task<Visitor> GetVisitorByIdAsync(int id)
    {
        const string query = "SELECT * FROM Visitors WHERE Id = @Id";
        var visitor = await _dapper.QuerySingleOrDefaultAsync<Visitor>(_dbConnection, query, new { Id = id });
        return visitor ?? throw new KeyNotFoundException($"Visitor with ID {id} not found");
    }
    public async Task<IEnumerable<Visitor>> GetAllVisitorsAsync()
    {
        const string query = "SELECT * FROM Visitors";
        return await _dapper.QueryAsync<IEnumerable<Visitor>>(_dbConnection, query);
    }

    public async Task<int> CreateVisitorAsync(Visitor visitor)
    {
        const string query = @"
                INSERT INTO Visitors (FirstName, LastName, Email, VisitDate, AccessPointId)
                VALUES (@FirstName, @LastName, @Email, @VisitDate, @AccessPointId);
                SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _dapper.QuerySingleAsync<int>(_dbConnection, query, visitor);
    }

    public async Task<bool> UpdateVisitorAsync(Visitor visitor)
    {
        const string query = @"
                UPDATE Visitors
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                    VisitDate = @VisitDate, AccessPointId = @AccessPointId
                WHERE Id = @Id";
        var result = await _dapper.ExecuteAsync(_dbConnection, query, visitor);
        return result > 0;
    }

    public async Task<bool> DeleteVisitorAsync(int id)
    {
        const string query = "DELETE FROM Visitors WHERE Id = @Id";
        var result = await _dapper.ExecuteAsync(_dbConnection, query, new { Id = id });
        return result > 0;
    }
}

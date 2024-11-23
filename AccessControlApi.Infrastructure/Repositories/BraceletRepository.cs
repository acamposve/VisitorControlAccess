using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using AccessControlApi.Infrastructure.Wrappers;
using Dapper;
using System.Data;

namespace AccessControlApi.Infrastructure.Repositories;

public class BraceletRepository : IBraceletRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDapperWrapper _dapper;

    public BraceletRepository(IDbConnection dbConnection, IDapperWrapper dapper)
    {
        _dbConnection = dbConnection;
        _dapper = dapper;
    }

    public async Task<Bracelet> GetByIdAsync(int id)
    {
        const string query = "SELECT * FROM Bracelets WHERE Id = @Id";
        return await _dapper.QuerySingleOrDefaultAsync<Bracelet>(_dbConnection, query, new { Id = id });
    }

    public async Task<IEnumerable<Bracelet>> GetAllAsync()
    {
        const string query = "SELECT * FROM Bracelets";
        return await _dapper.QueryAsync<IEnumerable<Bracelet>>(_dbConnection, query);
    }

    public async Task<int> CreateAsync(Bracelet bracelet)
    {
        const string query = @"INSERT INTO Bracelets (VisitorId, AccessPoints, CreatedAt, QrCode) 
                             VALUES (@VisitorId, @AccessPoints, @CreatedAt, @QrCode); 
                             SELECT CAST(SCOPE_IDENTITY() AS INT)";
        return await _dapper.QuerySingleAsync<int>(_dbConnection, query, bracelet);
    }

    public async Task UpdateAsync(Bracelet bracelet)
    {
        const string query = @"UPDATE Bracelets 
                             SET VisitorId = @VisitorId, 
                                 AccessPoints = @AccessPoints, 
                                 CreatedAt = @CreatedAt, 
                                 QrCode = @QrCode 
                             WHERE Id = @Id";
        await _dapper.ExecuteAsync(_dbConnection, query, bracelet);
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM Bracelets WHERE Id = @Id";
        await _dapper.ExecuteAsync(_dbConnection, query, new { Id = id });
    }
}
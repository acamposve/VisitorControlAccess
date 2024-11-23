using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using AccessControlApi.Infrastructure.Wrappers;
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace AccessControlApi.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDapperWrapper _dapper;

    public RoleRepository(IDbConnection dbConnection, IDapperWrapper dapper)
    {
        _dbConnection = dbConnection;
        _dapper = dapper;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        const string query = "SELECT * FROM Roles";
        return await _dapper.QueryAsync<IEnumerable<Role>>(_dbConnection, query);
    }

    public async Task<Role> GetByIdAsync(int id)
    {
        const string query = "SELECT * FROM Roles WHERE Id = @Id";
        var role = await _dapper.QuerySingleOrDefaultAsync<Role>(_dbConnection, query, new { Id = id });
        return role ?? throw new KeyNotFoundException($"Role with ID {id} not found");
    }

    public async Task<int> CreateAsync(Role role)
    {
        const string query = "INSERT INTO Roles (Name, Description) VALUES (@Name, @Description); SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _dapper.QuerySingleAsync<int>(_dbConnection, query, role);
    }

    public async Task UpdateAsync(Role role)
    {
        const string query = "UPDATE Roles SET Name = @Name, Description = @Description WHERE Id = @Id";
        await _dapper.ExecuteAsync(_dbConnection, query, role);
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM Roles WHERE Id = @Id";
        await _dapper.ExecuteAsync(_dbConnection, query, new { Id = id });
    }

    public async Task<Role> GetRoleByIdAsync(int roleId)
    {
        const string query = "SELECT * FROM Roles WHERE Id = @Id";
        var role = await _dapper.QuerySingleOrDefaultAsync<Role>(_dbConnection, query, new { Id = roleId });
        return role ?? throw new KeyNotFoundException($"Role with ID {roleId} not found");
    }
}
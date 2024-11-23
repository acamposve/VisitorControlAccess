using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using AccessControlApi.Infrastructure.Wrappers;
using System.Data;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDapperWrapper _dapper;

    public UserRepository(IDbConnection dbConnection, IDapperWrapper dapper)
    {
        _dbConnection = dbConnection;
        _dapper = dapper;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string query = "SELECT * FROM Users";
        return await _dapper.QueryAsync<IEnumerable<User>>(_dbConnection, query);
    }

    public async Task<User> GetByIdAsync(int id)
    {
        const string query = "SELECT * FROM Users WHERE Id = @Id";
        return await _dapper.QuerySingleOrDefaultAsync<User>(_dbConnection, query, new { Id = id });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        const string query = "SELECT * FROM Users WHERE Email = @Email";
        return await _dapper.QuerySingleOrDefaultAsync<User>(_dbConnection, query, new { Email = email });
    }

    public async Task<int> CreateAsync(User user)
    {
        const string query = "INSERT INTO Users (Name, Email, PasswordHash, RoleId) " +
                           "VALUES (@Name, @Email, @PasswordHash, @RoleId); " +
                           "SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _dapper.QuerySingleAsync<int>(_dbConnection, query, user);
    }

    public async Task UpdateAsync(User user)
    {
        const string query = "UPDATE Users SET Name = @Name, Email = @Email, " +
                           "PasswordHash = @PasswordHash, RoleId = @RoleId WHERE Id = @Id";
        await _dapper.ExecuteAsync(_dbConnection, query, user);
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM Users WHERE Id = @Id";
        await _dapper.ExecuteAsync(_dbConnection, query, new { Id = id });
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        const string query = "SELECT * FROM Users WHERE Id = @Id";
        return await _dapper.QuerySingleOrDefaultAsync<User>(_dbConnection, query, new { Id = userId });
    }

    public async Task<User> GetUserByCredentialsAsync(string email, string passwordHash)
    {
        const string query = "SELECT * FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";
        return await _dapper.QuerySingleOrDefaultAsync<User>(_dbConnection, query,
            new { Email = email, PasswordHash = passwordHash });
    }
}
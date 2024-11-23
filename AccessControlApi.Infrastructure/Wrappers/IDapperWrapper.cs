using System.Data;

namespace AccessControlApi.Infrastructure.Wrappers;

public interface IDapperWrapper
{
    Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null);
    Task<T> QueryAsync<T>(IDbConnection connection, string sql, object? param = null);
    Task<T> QuerySingleAsync<T>(IDbConnection connection, string sql, object? param = null);
    Task<int> ExecuteAsync(IDbConnection connection, string sql, object? param = null);
}

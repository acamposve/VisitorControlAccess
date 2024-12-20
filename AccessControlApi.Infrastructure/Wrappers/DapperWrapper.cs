﻿using Dapper;
using System.Data;

namespace AccessControlApi.Infrastructure.Wrappers;

public class DapperWrapper : IDapperWrapper
{
    public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null)
    {
        return await connection.QuerySingleOrDefaultAsync<T>(sql, param);
    }

    public async Task<T> QueryAsync<T>(IDbConnection connection, string sql, object? param = null)
    {
        return await connection.QueryFirstAsync<T>(sql, param);
    }

    public async Task<T> QuerySingleAsync<T>(IDbConnection connection, string sql, object? param = null)
    {
        return await connection.QuerySingleAsync<T>(sql, param);
    }

    public async Task<int> ExecuteAsync(IDbConnection connection, string sql, object? param = null)
    {
        return await connection.ExecuteAsync(sql, param);
    }
}

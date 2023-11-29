using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace ChatApp.Application.Services.ISqlService
{
    public class SqlService : Interfaces.ISqlService
    {
        private readonly string _connectionString;

        public SqlService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteStoredProcAsync(string storedProcName, object inputs = null, int commandTimeout = 600)
        {
            await using var connection = CreateSqlConnection();
            connection.Open();

            await using var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
            var result = await connection.ExecuteAsync(storedProcName, inputs, transaction, commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout);
            transaction.Commit();
            return result;
        }

        public async Task<int> ExecuteAsync(string query, object inputs = null, int commandTimeout = 600)
        {
            await using var connection = CreateSqlConnection();
            connection.Open();
            await using var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
            var result = await connection.ExecuteAsync(query, inputs, transaction, commandTimeout);
            transaction.Commit();
            return result;
        }

        public int Execute(string query, object inputs = null, int commandTimeout = 600)
        {
            using var connection = CreateSqlConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
            var result = connection.Execute(query, inputs, transaction, commandTimeout);
            transaction.Commit();
            return result;
        }

        private SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}

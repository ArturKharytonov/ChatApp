namespace ChatApp.Application.Services.ISqlService.Interfaces;

public interface ISqlService
{
    Task<int> ExecuteStoredProcAsync(string storedProcName, object inputs = null, int commandTimeout = 600);
    Task<int> ExecuteAsync(string query, object inputs = null, int commandTimeout = 600);
    int Execute(string query, object inputs = null, int commandTimeout = 600);
}
using ChatApp.Application.Services.ISqlService;
using ChatApp.Application.Services.ISqlService.Interfaces;

namespace ChatApp.IntegrationTests;

public class TestBase
{
    protected HttpClient Client;
    protected AuthenticationHelper AuthHelper;
    protected ChatWebApplicationFactory Factory;
    protected ISqlService _sqlService;
    public TestBase()
    {
        Factory = new ChatWebApplicationFactory();
        Client = Factory.CreateClient();
        AuthHelper = new AuthenticationHelper(Client);
        _sqlService = new SqlService(Factory.ConnectionString);
    }

    protected async Task<bool> CheckIfRecordExists(string table, string column, object value)
    {
        return await _sqlService.ExecuteScalarAsync<bool>(SqlScripts.SqlScripts.ExistsScript(table, column), new() { { "@value", value } });
    }
}
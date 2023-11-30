namespace ChatApp.IntegrationTests.SqlScripts;

public static class SqlScripts
{
    public static string ExistsScript(string tableName, string columnName)
    {
        return $"""
                   SELECT CASE WHEN EXISTS (SELECT TOP 1 *
                                FROM [dbo].[{tableName}] 
                                WHERE [{columnName}] = @value) 
                   THEN CAST (1 AS BIT) 
                   ELSE CAST (0 AS BIT) END
                   """;
    }
}
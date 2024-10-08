using ChatApp.Application.Services.QueryBuilder;

namespace ChatApp.Tests.Application.Tests;

public class QueryBuilderTests
{
    [Theory]
    [InlineData("test", new[] { "Name", "Description" }, true)]
    [InlineData("123", new[] { "Id", "Value" }, true)]
    public void SearchQuery_ShouldGenerateCorrectExpression(string searchValue, string[] properties, bool expectedResult)
    {
        // Arrange
        var queryBuilder = new QueryBuilder<TestEntity>();

        // Act
        var expression = queryBuilder.SearchQuery(searchValue, properties);

        // Assert
        var compiledExpression = expression.Compile();
        var entity = new TestEntity { Id = 123, Value = 456, Name = "testName", Description = "testDescription" };
        Assert.Equal(expectedResult, compiledExpression(entity));
    }

    [Theory]
    [InlineData("Id", true, new[] { 1, 2, 3 })]
    [InlineData("Id", false, new[] { 3, 2, 1 })]
    public void OrderByQuery_ShouldOrderByCorrectly(string orderByValue, bool orderByType, int[] expectedOrder)
    {
        // Arrange
        var queryBuilder = new QueryBuilder<TestEntity>();
        var source = expectedOrder
            .Select(id => new TestEntity { Id = id })
            .AsQueryable();

        // Act
        var result = queryBuilder
            .OrderByQuery(source, orderByValue, orderByType)
            .ToList();

        // Assert
        for (var i = 0; i < expectedOrder.Length; i++)
            Assert.Equal(expectedOrder[i], result[i].Id);
    }

    private class TestEntity
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
    }
}
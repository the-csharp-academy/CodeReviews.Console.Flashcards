using CodeReviews.Console.Flashcards;
using CodeReviews.Console.Flashcards.Tests;
using Dapper;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Flashcards.Tests;

[TestFixture]
[Category("Integration")]
[NonParallelizable]
public sealed class StacksRepoTests
{
    private TestDatabase _database;
    private IStacksRepo _repository;

    [SetUp]
    public void SetUp()
    {
        _database = new TestDatabase();
        _database.Initialize();

        _repository = new StacksRepo(_database.connectionFactory);
    }

    [TearDown]
    public void TearDown() => _database.Dispose();

    [Test]
    public void Add_WhenNameIsValid_InsertsStack()
    {
        _repository.Add("German");

        using (var connection = _database.OpenConnection())
        {
            string sql = @"
                SELECT Name
                FROM dbo.Stacks
                WHERE Name = @Name;
            ";
            string? savedName = connection.QuerySingleOrDefault<string>(sql, new { Name = "German" });

            Assert.That(savedName, Is.EqualTo("German"));

        }

    }
    [Test]
    public void GetAll_WhenStacksExist_ReturnsAllStacks()
    {
        InsertStack("German");
        InsertStack("C#");

        List<CardStack> result = _repository.GetAll();

        string[] names = result.Select(stack => stack.Name).ToArray();

        Assert.That(names, Is.EquivalentTo(new[] { "German", "C#" }));
    }
    [Test]
    public void GetStackByName_WhenStackExists_ReturnsMatchingStack()
    {
        long stackId = InsertStack("German");

        CardStack? result = _repository.GetStackByName("German");

        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result!.StackId, Is.EqualTo(stackId));
            Assert.That(result.Name, Is.EqualTo("German"));
        });
    }

    [Test]
    public void GetStackByName_WhenStackDoesNotExist_ReturnsNull()
    {
        CardStack? result = _repository.GetStackByName("Missing");
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Update_WhenStackExists_ChangesItsName()
    {
        long stackId = InsertStack("Old name");

        _repository.Update(stackId, "New name");

        using (var connection = _database.OpenConnection())
        {
            string sql = @"
                SELECT Name
                FROM dbo.Stacks
                WHERE StackId = @StackId;
            ";
            string savedName = connection.QuerySingle<string>(sql, new { StackId = stackId });

            Assert.That(savedName, Is.EqualTo("New name"));
        }

    }

    [Test]
    public void Delete_WhenStackExists_RemovesIt()
    {
        long stackId = InsertStack("German");

        _repository.Delete(stackId);

        using (var connection = _database.OpenConnection())
        {
            string sql = @"
                SELECT COUNT(*)
                FROM dbo.Stacks
                WHERE StackId = @StackId;
            ";

            int remainingCount = connection.QuerySingle<int>(sql, new { StackId = stackId });

            Assert.That(remainingCount, Is.Zero);

        }
    }

    [Test]
    public void Add_WhenNameAlreadyExists_ThrowsSqlException()
    {
        _repository.Add("German");
        Assert.Throws<SqlException>(() => _repository.Add("German"));
    }

    private long InsertStack(string name)
    {
        using (var connection = _database.OpenConnection())
        {
            string sql = @"
                INSERT INTO dbo.Stacks (Name)
                VALUES (@Name);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            ";

            return connection.QuerySingle<long>(sql, new { Name = name });
        }

    }

}
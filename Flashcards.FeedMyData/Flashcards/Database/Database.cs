using Dapper;

namespace Flashcards;

public sealed class Database
{
    private readonly DbConnectionProvider _connection;
    private const string flashcardsTableName = "flash.flashcards";
    private const string stacksTableName = "flash.stacks";
    private const string studiesTableName = "flash.studies";

    public Database(DbConnectionProvider connectionProvider)
    {
        _connection = connectionProvider;
    }

    // ------------------------
    // ---- SETUP METHODS -----
    // ------------------------

    internal void CreateDB()
    {
        string sql = @$"IF db_ID('tcsa') IS NULL 
                            EXEC(N'CREATE DATABASE [tcsa]')";

        _connection.Execute(connection => connection.Execute(sql));
    }

    internal void CreateSchema()
    {
        string sql = @$"IF SCHEMA_id(N'flash') IS NULL
                            EXEC(N'CREATE SCHEMA [flash]')";

        _connection.Execute(connection => connection.Execute(sql));
    }

    internal int GetAppTableCount()
    {
        // const string sql = """
        // SELECT
        //     CASE WHEN OBJECT_ID(N'flash.stacks', N'U') IS NOT NULL THEN 1 ELSE 0 END
        //   + CASE WHEN OBJECT_ID(N'flash.flashcards', N'U') IS NOT NULL THEN 1 ELSE 0 END;
        // """;

        const string sql = """
                    SELECT COUNT(*) AS TableCount
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'flash'
                    AND TABLE_TYPE = 'BASE TABLE';
                    """;

        return _connection.Execute(connection => connection.ExecuteScalar<int>(sql));
    }

    internal void CreateTables()
    {
        CreateTableStacks();
        CreateTableFlashcards();
        CreateTableStudies();
    }

    private void CreateTableStacks()
    {
        string sql = """
            IF OBJECT_ID(N'tcsa.flash.stacks', N'U') IS NULL
                CREATE TABLE tcsa.flash.stacks(
                    stackId INT IDENTITY(1, 1) PRIMARY KEY,
                    name NVARCHAR(200) NOT NULL UNIQUE)
            """;

        _connection.Execute(connection => connection.Execute(sql));
    }

    private void CreateTableFlashcards()
    {
        string sql = """
            IF OBJECT_ID(N'flash.flashcards', N'U') IS NULL
                CREATE TABLE flash.flashcards (
                    flashcardId INT IDENTITY(1,1) PRIMARY KEY,
                    recto NVARCHAR(MAX),
                    verso NVARCHAR(MAX),
                    stackId INT NOT NULL,
                    FOREIGN KEY (stackId) REFERENCES flash.stacks(stackId)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE)
            """;

        _connection.Execute(connection => connection.Execute(sql));
    }

    private void CreateTableStudies()
    {
        string sql = """
            IF OBJECT_ID(N'flash.studies', N'U') IS NULL
                CREATE TABLE flash.studies (
                    studyId INT IDENTITY(1,1) PRIMARY KEY,
                    stackId INT NOT NULL,
                    date DATE,
                    score INT,
                    scoreMax INT,
                    FOREIGN KEY (stackId) REFERENCES flash.stacks(stackId)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE)
            """;

        _connection.Execute(connection => connection.Execute(sql));
    }

    // ------------------------
    // ---- CRUD METHODS -----
    // ------------------------

    internal void AddFlashcard(int stackId, string recto, string verso)
    {
        string sql = @$"INSERT INTO {flashcardsTableName} (stackId, recto, verso) VALUES (@stackId, @recto, @verso )";

        _connection.Execute(connection => connection.Execute(sql, new { stackId, recto, verso, }));
    }

    internal void AddStack(string name)
    {
        string sql = @$"INSERT INTO {stacksTableName} (name) VALUES (@name)";

        _connection.Execute(connection => connection.Execute(sql, new { name }));
    }

    internal void AddStudy(int stackId, DateOnly date, int score, int scoreMax)
    {
        string sql = @$"INSERT INTO {studiesTableName} (stackId, date, score, scoreMax) VALUES (@stackId, @date, @score, @scoreMax)";

        _connection.Execute(connection => connection.Execute(sql, new { stackId, date, score, scoreMax }));
    }

    internal void DeleteStack(Stack stack)
    {
        string sql = @$"DELETE FROM {stacksTableName} WHERE stackId = @stackId";

        _connection.Execute(connection => connection.Execute(sql, new { stack.StackId }));
    }

    internal void DeleteFlashcard(Flashcard f)
    {
        string sql = @$"DELETE FROM {flashcardsTableName} WHERE flashcardId = @flashcardId";

        _connection.Execute(connection => connection.Execute(sql, new { f.FlashcardId }));
    }

    internal void UpdateStack(Stack stack)
    {
        string sql = @$"UPDATE {stacksTableName} SET name = @name WHERE stackId = @stackId";

        _connection.Execute(connection => connection.Execute(sql, new { stack.Name, stack.StackId }));
    }

    internal void UpdateFlashcard(Flashcard f)
    {
        string sql = @$"UPDATE {flashcardsTableName} SET recto = @recto, verso = @verso WHERE flashcardId = @flashcardId";

        _connection.Execute(connection => connection.Execute(sql, new { f.Recto, f.Verso, f.FlashcardId }));
    }

    // ------------------------
    // ---- QUERY METHODS -----
    // ------------------------

    internal List<Study> QueryAllFromStudies()
    {
        string sql = @$"SELECT * FROM {studiesTableName} ORDER BY date DESC";
        return _connection.Execute(connection => connection.Query<Study>(sql).ToList());
    }

    internal List<Flashcard> QueryAllFromFlashcards()
    {
        string sql = @$"SELECT * FROM {flashcardsTableName} ORDER BY recto ASC";
        return _connection.Execute(connection => connection.Query<Flashcard>(sql).ToList());
    }

    internal List<Flashcard> QueryFlashcardsFromStack(Stack stack)
    {
        string sql = @$"SELECT * FROM {flashcardsTableName} WHERE stackId = @stackId ORDER BY recto ASC";
        return _connection.Execute(connection => connection.Query<Flashcard>(sql, new { stack.StackId }).ToList());
    }

    internal List<Stack> QueryAllFromStacks()
    {
        string sql = @$"SELECT * FROM {stacksTableName} ORDER BY name ASC";
        return _connection.Execute(connection => connection.Query<Stack>(sql).ToList());
    }

    internal Flashcard? QueryOneFlashcard()
    {
        string sql = @$"SELECT TOP (1) * FROM {flashcardsTableName}";
        return _connection.Execute(connection => connection.QueryFirstOrDefault<Flashcard>(sql));
    }

    internal Stack? QueryOneStack()
    {
        string sql = @$"SELECT TOP (1) * FROM {stacksTableName}";
        return _connection.Execute(connection => connection.QueryFirstOrDefault<Stack>(sql));
    }

    internal Stack? QueryNewestStack()
    {
        string sql = @$"SELECT TOP (1) * FROM {stacksTableName} ORDER BY stackId DESC";
        return _connection.Execute(connection => connection.QueryFirstOrDefault<Stack>(sql));
    }

    internal List<StudyDto> QueryMergeStudiesAndStacksNoId()
    {
        string sql = @$"SELECT studies.date, stacks.name, studies.score, studies.scoreMax 
                        FROM flash.studies
                        LEFT JOIN flash.stacks
                        ON flash.studies.stackId = flash.stacks.stackId
                        ORDER BY flash.studies.date DESC";

        return _connection.Execute(connection => connection.Query<StudyDto>(sql).ToList());
    }
    // ------------------------
    // ---- DELETE ALL---------
    // ------------------------

    internal void DropAll()
    {
        // This fails if the schema contains non-table objects. 
        // To drop all objects (tables, views, procedures) in a schema, you must generate DROP statements for each object type
        string dropStack = "DROP TABLE IF EXISTS flash.flashcards";

        _connection.Execute(connection => connection.Execute(dropStack));

        string sql = """
            DECLARE @SchemaName NVARCHAR(128) = 'flash';
        DECLARE @DropTables NVARCHAR(MAX) = '';

        --Drop Tables
        SELECT @DropTables += 'DROP TABLE [' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']; '
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = @SchemaName
            AND TABLE_TYPE = 'BASE TABLE';

        EXEC sp_executesql @DropTables;
        """;

        _connection.Execute(connection => connection.Execute(sql));
    }
}
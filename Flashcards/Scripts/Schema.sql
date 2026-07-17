SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;
        IF OBJECT_ID(N'dbo.Stacks', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Stacks
                (
                    StackId INT IDENTITY(1, 1) NOT NULL,
                    Name NVARCHAR(100) NOT NULL,

                    CONSTRAINT PK_Stacks
                        PRIMARY KEY (StackId),

                    CONSTRAINT UQ_Stacks_Name
                        UNIQUE (Name),

                    CONSTRAINT CK_Stacks_Name_NotEmpty
                        CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
                );
            END;


            IF OBJECT_ID(N'dbo.Flashcards', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Flashcards
                    (
                        FlashcardId INT IDENTITY(1, 1) NOT NULL,
                        StackId INT NOT NULL,
                        Question NVARCHAR(500) NOT NULL,
                        Answer NVARCHAR(500) NOT NULL,

                        CONSTRAINT PK_Flashcards
                            PRIMARY KEY (FlashcardId),

                        CONSTRAINT FK_Flashcards_Stacks
                            FOREIGN KEY (StackId)
                            REFERENCES dbo.Stacks(StackId)
                            ON DELETE CASCADE,

                        CONSTRAINT CK_Flashcards_Question_NotEmpty
                            CHECK (LEN(LTRIM(RTRIM(Question))) > 0),

                        CONSTRAINT CK_Flashcards_Answer_NotEmpty
                            CHECK (LEN(LTRIM(RTRIM(Answer))) > 0)
                    );
                END;

            IF NOT EXISTS
            (
                SELECT 1
                FROM sys.indexes
                WHERE name = N'IX_Flashcards_StackId'
                AND object_id = OBJECT_ID(N'dbo.Flashcards')
            )
            BEGIN
                CREATE INDEX IX_Flashcards_StackId
                    ON dbo.Flashcards(StackId);
            END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    
    THROW;
END CATCH;

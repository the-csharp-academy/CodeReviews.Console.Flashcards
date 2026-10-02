using Dapper;
using Flashcards.DTOs;
using Flashcards.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Flashcards.Repositories
{
    public class StudySessionRepository
    {
        private readonly string connectionString;
        private readonly string table = "sessions";
        public StudySessionRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private bool Execute(Func<IDbConnection, bool> action)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return action(db);
            }
        }

        public bool CreateTable()
        {
            return Execute(db =>
            {
                var query =
                @$"IF NOT EXISTS 
                    (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{table}') AND type in (N'U'))
                    CREATE TABLE {table}
                    (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        CardStackId INT,
                        Time DATETIME,
                        Score INT,
                        FOREIGN KEY (CardStackId) REFERENCES stacks (Id) ON DELETE CASCADE
                    );";
                return db.Execute(query) != 0;
            });
        }

        public bool CreateSession(StudySessionEntity session)
        {
            return Execute((db) =>
            {
                var query =
                @$"INSERT INTO {table}(CardStackId, Time, Score) 
                    VALUES(@CardStackId, @Time, @Score);";
                return db.Execute(query, session) > 0;
            });
        }

        public List<StudySessionEntity> ReadSessions()
        {
            using(IDbConnection db = new SqlConnection(connectionString))
            {
                var query =
                    $@"SELECT * FROM {table};";
                return db.Query<StudySessionEntity>(query).ToList();
            }
        }

        public List<MonthlySessionDTO> GetMonthlySessions(int year)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var query =
                    $@"SELECT StackName,
                    [1] AS January,
                    [2] AS February,
                    [3] AS March,
                    [4] AS April,
                    [5] AS May,
                    [6] AS June,
                    [7] AS July,
                    [8] AS August,
                    [9] AS September,
                    [10] AS October,
                    [11] AS November,
                    [12] AS December
                    FROM 
                    (
                        SELECT stacks.Id AS StackId,
                            stacks.Name AS StackName,
                            sessions.Id AS SessionId,
                            MONTH(sessions.Time) AS SessionMonth
                        FROM stacks
                        LEFT JOIN sessions
                            ON sessions.CardStackId = stacks.Id
                            AND sessions.Time >= DATEFROMPARTS(@Year, 1, 1)
                            AND sessions.Time < DATEFROMPARTS(@Year + 1, 1, 1)
                    ) AS source
                    PIVOT
                    (
                        COUNT(SessionId)
                        FOR SessionMonth IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                    ) AS report ORDER BY StackName;";
                return db.Query<MonthlySessionDTO>(query, new { Year = year }).ToList();
            }
        }

        public List<AverageScoreDTO> GetAverageScores(int month, int year)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var query =
                    $@"SELECT stacks.Name AS StackName,
                            MONTH(sessions.Time) AS SessionMonth,
                            AVG(CAST(sessions.Score AS FLOAT)) AS AverageScore
                        FROM stacks
                        LEFT JOIN sessions
                            ON sessions.CardStackId = stacks.Id
                            AND MONTH(sessions.Time) = @Month
                            AND YEAR(sessions.Time) = @Year
                        GROUP BY stacks.Name, MONTH(sessions.Time);";
                return db.Query<AverageScoreDTO>(query, new { Month = month, Year = year }).ToList();
            }
        }
    }
}
using Dapper;
using Flashcards.m1chael888.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.m1chael888.Repositories;

public interface ICardRepository
{
    void Create(CardModel card);
    List<CardModel> Read(int stackId);
    void Update(CardModel card);
    void Delete(int cardId);
}
public class CardRepository : ICardRepository
{
    private readonly string _connectionString;
    public CardRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Create(CardModel card)
    {
        var sql = @"INSERT INTO Cards (Front, Back, StackId) VALUES (@Front, @Back, @StackId)";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql, new
            {
                Front = card.Front,
                Back = card.Back,
                StackId = card.StackId
            });
        }
    }

    public List<CardModel> Read(int stackId)
    {
        var cards = new List<CardModel>();
        var sql = @"SELECT * FROM Cards WHERE StackId = @StackId";

        using (var connection = new SqlConnection(_connectionString))
        {
            cards = connection.Query<CardModel>(sql, new
            {
                StackId = stackId
            }).ToList();
        }
        return cards;
    }

    public void Update(CardModel card)
    {
        var sql = @"UPDATE Cards SET Front = @Front, Back = @Back WHERE CardId = @CardId";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql, new
            {
                Front = card.Front,
                Back = card.Back,
                CardId = card.CardId
            });
        }
    }

    public void Delete(int cardId)
    {
        var sql = @"DELETE FROM Cards WHERE CardId = @CardId";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql, new
            {
                CardId = cardId
            });
        }
    }
}

namespace Flashcards;

public class DatabaseAccessException : Exception
{

    string message;

    // Default constructor
    public DatabaseAccessException() : base() { }

    // Constructor with message
    public DatabaseAccessException(string message) : base(message) { }

    // Constructor with message and inner exception
    public DatabaseAccessException(string message, Exception innerException) : base(message, innerException) { }
}


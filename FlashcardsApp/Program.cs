namespace FlashcardsApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string connectionString = "Server=.;Database=Flashcard;Trusted_Connection=True;TrustServerCertificate=True;";
            Menu menu = new Menu(connectionString);
        }
    }
}
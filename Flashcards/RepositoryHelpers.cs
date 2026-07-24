internal static class RepositoryHelpers
{
    public static bool TryExecute(Action operation, out Exception? ex)
    {
        if (operation == null)
            throw new ArgumentNullException();

        try
        {
            operation();
            ex = null;
            return true;
        }
        catch (Exception e)
        {
            ex = e;
            return false;
        }
    }
}
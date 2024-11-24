namespace Template.CommandServer.Service;

public sealed class DataService
{
    private readonly Lock sync = new();

    private int storedValue;

    public void UpdateValue(int value)
    {
        lock (sync)
        {
            storedValue = value;
        }
    }

    public int QueryValue()
    {
        lock (sync)
        {
            return storedValue;
        }
    }
}

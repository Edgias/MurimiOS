namespace EdgIQ.MurimiOS.Worker;

public sealed class ProviderIngestionOptions
{
    public bool Enabled { get; set; } = true;
    public int ScheduleBatchSize { get; set; } = 100;
    public int MaximumAttempts { get; set; } = 8;
    public int LockSeconds { get; set; } = 300;
    public int PollMilliseconds { get; set; } = 5_000;

    public void Validate()
    {
        if (ScheduleBatchSize is < 1 or > 1_000 || MaximumAttempts is < 1 or > 20
            || LockSeconds is < 10 or > 3_600 || PollMilliseconds is < 100 or > 300_000)
            throw new InvalidOperationException("Provider ingestion configuration is invalid.");
    }
}

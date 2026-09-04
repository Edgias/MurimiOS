using Xunit;

// Every integration class starts an API host against the same PostgreSQL database.
// Run those hosts serially; request concurrency is exercised explicitly by ApiLoadTests.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

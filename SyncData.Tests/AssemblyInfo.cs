using Xunit;

// Console redirection tests mutate global state, so run tests sequentially.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

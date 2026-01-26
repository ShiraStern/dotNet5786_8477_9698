using System.Threading.Tasks;

namespace PL.Helpers;

/// <summary>
/// A helper class that synchronizes load operations.
/// It ensures that only one load runs at a time,
/// and tracks whether a restart is required if another request arrives.
/// </summary>
internal class ObserverMutex
{
    private const int DELAY_MILLISECONDS = 100;

    private bool _isLoadInProgress = false;

    private bool _isRestartRequested = false;

    internal bool CheckAndSetLoadInProgressOrRestartRequired()
    {
        lock (this)
        {
            _isRestartRequested = _isLoadInProgress;
            _isLoadInProgress = true;
            return _isRestartRequested;
        }
    }

    internal async Task<bool> UnsetLoadInProgressAndCheckRestartRequested()
    {
        await Task.Delay(DELAY_MILLISECONDS);

        lock (this)
        {
            _isLoadInProgress = false;
            return _isRestartRequested;
        }
    }
}

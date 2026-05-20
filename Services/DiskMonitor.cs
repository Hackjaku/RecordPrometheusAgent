using PrometheusAgent.Configuration;
using PrometheusAgent.Metrics;

namespace PrometheusAgent.Services;

public sealed class DiskMonitor : IMonitor {
    private readonly DiskConfig _config;

    public DiskMonitor(DiskConfig config) {
        _config = config;
    }

    public void Start(CancellationToken cancellationToken) {
        _ = Task.Run(() => RunAsync(cancellationToken), cancellationToken);
    }

    private async Task RunAsync(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            UpdateMetrics();

            await Task.Delay(TimeSpan.FromSeconds(_config.IntervalSeconds), cancellationToken);
        }
    }

    private void UpdateMetrics() {
        var drives = DriveInfo.GetDrives()
            .Where(d => d.IsReady);

        if (_config.Drives.Count > 0) {
            drives = drives.Where(d =>
                _config.Drives.Any(configured =>
                    string.Equals(
                        NormalizeDrive(configured),
                        NormalizeDrive(d.Name),
                        StringComparison.OrdinalIgnoreCase)));
        }

        foreach (var drive in drives) {
            var total = drive.TotalSize;
            var free = drive.AvailableFreeSpace;
            var used = total - free;
            var usedPercent = total > 0
                ? used * 100.0 / total
                : 0;

            MetricDefinitions.DiskTotal
                .WithLabels(drive.Name)
                .Set(total);

            MetricDefinitions.DiskFree
                .WithLabels(drive.Name)
                .Set(free);

            MetricDefinitions.DiskUsedPercent
                .WithLabels(drive.Name)
                .Set(usedPercent);
        }
    }

    private static string NormalizeDrive(string value) {
        return value.Trim().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}

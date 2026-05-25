using System.Diagnostics;
using System.Runtime.InteropServices;
using PrometheusAgent.Configuration;
using PrometheusAgent.Metrics;

namespace PrometheusAgent.Services;

public sealed class CpuMonitor : IMonitor {
    private readonly CpuConfig _config;
    private readonly PerformanceCounter? _windowsCpuCounter;

    private LinuxCpuSnapshot? _lastLinuxSnapshot;

    public CpuMonitor(CpuConfig config) {
        _config = config;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            try {
                _windowsCpuCounter = new PerformanceCounter(
                    "Processor",
                    "% Processor Time",
                    "_Total");

                _windowsCpuCounter.NextValue();
            }
            catch {
                _windowsCpuCounter = null;
            }
        }
    }

    public void Start(CancellationToken cancellationToken) {
        _ = Task.Run(() => RunAsync(cancellationToken), cancellationToken);
    }

    private async Task RunAsync(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            UpdateMetrics();

            await Task.Delay(
                TimeSpan.FromSeconds(_config.IntervalSeconds),
                cancellationToken);
        }
    }

    private void UpdateMetrics() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            UpdateWindowsMetrics();
            return;
        }

        UpdateLinuxMetrics();
    }

    private void UpdateWindowsMetrics() {
        if (_windowsCpuCounter is null)
            return;

#pragma warning disable CA1416 // Validate platform compatibility
        var value = _windowsCpuCounter.NextValue();
#pragma warning restore CA1416 // Validate platform compatibility

        MetricDefinitions.SystemCpuUsagePercent.Set(value);
    }

    private void UpdateLinuxMetrics() {
        var current = ReadLinuxCpuSnapshot();

        if (_lastLinuxSnapshot is null) {
            _lastLinuxSnapshot = current;
            return;
        }

        var previous = _lastLinuxSnapshot.Value;

        var idleDelta = current.Idle - previous.Idle;
        var totalDelta = current.Total - previous.Total;

        if (totalDelta <= 0)
            return;

        var usage = (1.0 - idleDelta / (double)totalDelta) * 100.0;

        MetricDefinitions.SystemCpuUsagePercent.Set(usage);

        _lastLinuxSnapshot = current;
    }

    private static LinuxCpuSnapshot ReadLinuxCpuSnapshot() {
        var line = File.ReadLines("/proc/stat")
            .First(x => x.StartsWith("cpu "));

        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(long.Parse)
            .ToArray();

        var idle = parts[3] + parts[4];
        var total = parts.Sum();

        return new LinuxCpuSnapshot(total, idle);
    }

    private readonly record struct LinuxCpuSnapshot(long Total, long Idle);
}

using PrometheusAgent.Configuration;
using PrometheusAgent.Metrics;

namespace PrometheusAgent.Services;

public sealed class RamMonitor : IMonitor {
  private readonly RamConfig _config;

  public RamMonitor(RamConfig config) {
    _config = config;
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

  private static void UpdateMetrics() {
    var memInfo = File.ReadAllLines("/proc/meminfo");

    long totalKb = ReadMemInfoValue(memInfo, "MemTotal:");
    long availableKb = ReadMemInfoValue(memInfo, "MemAvailable:");

    long totalBytes = totalKb * 1024;
    long availableBytes = availableKb * 1024;
    long usedBytes = totalBytes - availableBytes;

    double usedPercent = totalBytes > 0
        ? usedBytes * 100.0 / totalBytes
        : 0;

    MetricDefinitions.SystemRamTotal.Set(totalBytes);
    MetricDefinitions.SystemRamAvailable.Set(availableBytes);
    MetricDefinitions.SystemRamUsed.Set(usedBytes);
    MetricDefinitions.SystemRamUsedPercent.Set(usedPercent);
  }

  private static long ReadMemInfoValue(string[] lines, string key) {
    var line = lines.FirstOrDefault(x => x.StartsWith(key));

    if (line is null)
      return 0;

    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    return long.TryParse(parts[1], out var value)
        ? value
        : 0;
  }
}

using System.Diagnostics;
using PrometheusAgent.Configuration;
using PrometheusAgent.Metrics;

namespace PrometheusAgent.Services;

public sealed class ProcessMonitor : IMonitor {
  private readonly ProcessConfig _config;

  public ProcessMonitor(ProcessConfig config) {
    _config = config;
  }

  public void Start(CancellationToken cancellationToken) {
    _ = Task.Run(() => RunAsync(cancellationToken), cancellationToken);
  }

  private async Task RunAsync(CancellationToken cancellationToken) {
    while (!cancellationToken.IsCancellationRequested) {
      var process = Process.GetCurrentProcess();

      MetricDefinitions.AgentProcessRamUsage.Set(process.WorkingSet64);

      await Task.Delay(TimeSpan.FromSeconds(_config.IntervalSeconds), cancellationToken);
    }
  }
}

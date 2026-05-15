using System.Net.NetworkInformation;
using PrometheusAgent.Configuration;
using PrometheusAgent.Metrics;

namespace PrometheusAgent.Services;

public sealed class PingMonitor : IMonitor {
  private readonly PingConfig _config;

  public PingMonitor(PingConfig config) {
    _config = config;
  }

  public void Start(CancellationToken cancellationToken) {
    _ = Task.Run(() => RunAsync(cancellationToken), cancellationToken);
  }

  private async Task RunAsync(CancellationToken cancellationToken) {
    while (!cancellationToken.IsCancellationRequested) {
      foreach (var target in _config.Targets) {
        await CheckTargetAsync(target, cancellationToken);
      }

      await Task.Delay(TimeSpan.FromSeconds(_config.IntervalSeconds), cancellationToken);
    }
  }

  private static async Task CheckTargetAsync(PingTargetConfig target, CancellationToken cancellationToken) {
    try {
      using var ping = new Ping();

      var reply = await ping.SendPingAsync(target.Host, 3000);

      if (reply.Status == IPStatus.Success) {
        MetricDefinitions.PingStatus
            .WithLabels(target.Name, target.Host)
            .Set(1);

        MetricDefinitions.PingLatency
            .WithLabels(target.Name, target.Host)
            .Set(reply.RoundtripTime);
      }
      else {
        MetricDefinitions.PingStatus
            .WithLabels(target.Name, target.Host)
            .Set(0);

        MetricDefinitions.PingLatency
            .WithLabels(target.Name, target.Host)
            .Set(-1);
      }
    }
    catch {
      MetricDefinitions.PingStatus
          .WithLabels(target.Name, target.Host)
          .Set(0);

      MetricDefinitions.PingLatency
          .WithLabels(target.Name, target.Host)
          .Set(-1);
    }
  }
}

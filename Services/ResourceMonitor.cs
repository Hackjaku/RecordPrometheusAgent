using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Prometheus;

using PrometheusAgent.Metrics;

namespace PrometheusAgent.Services;

public class ResourceMonitor {
    public async Task StartAsync(CancellationToken cancellationToken) {
        _ = Task.Run(async () => {
            while (!cancellationToken.IsCancellationRequested) {
                UpdateMetrics();
                await Task.Delay(5000, cancellationToken);
            }
        });
    }

    private void UpdateMetrics() {
        var ram = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
        MetricDefinitions.RamUsage.Set(ram);
    }
}
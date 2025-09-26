using Prometheus;
using PrometheusAgent.Services;

var server = new MetricServer(port: 9090);
server.Start();

var resourceMonitor = new ResourceMonitor();
await resourceMonitor.StartAsync(CancellationToken.None);

Console.WriteLine("Prometheus metrics server is running on http://localhost:9090/metrics");
await Task.Delay(Timeout.Infinite);
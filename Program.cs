using Microsoft.Extensions.Configuration;

using Prometheus;

using PrometheusAgent.Services;
using PrometheusAgent.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("config.json", optional: false, reloadOnChange: true)
    .Build();

var agentConfig = configuration.Get<AgentConfig>()
    ?? throw new InvalidOperationException("Invalid configuration");

var server = new MetricServer(port: agentConfig.Prometheus.Port);
server.Start();

using var cts = new CancellationTokenSource();

var monitors = new List<IMonitor>();

if (agentConfig.Monitoring.Ping.Enabled) {
    monitors.Add(new PingMonitor(agentConfig.Monitoring.Ping));
}

if (agentConfig.Monitoring.Disk.Enabled) {
    monitors.Add(new DiskMonitor(agentConfig.Monitoring.Disk));
}

if (agentConfig.Monitoring.Process.Enabled) {
    monitors.Add(new ProcessMonitor(agentConfig.Monitoring.Process));
}

if (agentConfig.Monitoring.Ram.Enabled) {
    monitors.Add(new RamMonitor(agentConfig.Monitoring.Ram));
}

if (agentConfig.Monitoring.Mysql.Enabled) {
    monitors.Add(new MysqlMonitor(agentConfig.Monitoring.Mysql));
}

foreach (var monitor in monitors) {
    monitor.Start(cts.Token);
}

Console.WriteLine($"Prometheus metrics server running on http://localhost:{agentConfig.Prometheus.Port}/metrics");

await Task.Delay(Timeout.Infinite, cts.Token);

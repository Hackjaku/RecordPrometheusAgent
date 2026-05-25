using Prometheus;

namespace PrometheusAgent.Metrics;

public static class MetricDefinitions {
    public static readonly Gauge AgentProcessRamUsage = Prometheus.Metrics.CreateGauge(
        "agent_process_ram_usage_bytes",
        "RAM used by the Prometheus agent process.");

    public static readonly Gauge PingStatus = Prometheus.Metrics.CreateGauge(
        "ping_status",
        "Ping status. 1 = up, 0 = down.",
        new GaugeConfiguration { LabelNames = new[] { "name", "target" } });

    public static readonly Gauge PingLatency = Prometheus.Metrics.CreateGauge(
        "ping_latency_ms",
        "Ping latency in milliseconds.",
        new GaugeConfiguration { LabelNames = new[] { "name", "target" } });

    public static readonly Gauge DiskTotal = Prometheus.Metrics.CreateGauge(
        "disk_total_bytes",
        "Total disk space in bytes.",
        new GaugeConfiguration { LabelNames = new[] { "drive" } });

    public static readonly Gauge DiskFree = Prometheus.Metrics.CreateGauge(
        "disk_free_bytes",
        "Free disk space in bytes.",
        new GaugeConfiguration { LabelNames = new[] { "drive" } });

    public static readonly Gauge DiskUsedPercent = Prometheus.Metrics.CreateGauge(
        "disk_used_percent",
        "Used disk space percentage.",
        new GaugeConfiguration { LabelNames = new[] { "drive" } });

    public static readonly Gauge SystemRamTotal = Prometheus.Metrics.CreateGauge(
        "system_ram_total_bytes",
        "Total system RAM in bytes.");

    public static readonly Gauge SystemRamAvailable = Prometheus.Metrics.CreateGauge(
        "system_ram_available_bytes",
        "Available system RAM in bytes.");

    public static readonly Gauge SystemRamUsed = Prometheus.Metrics.CreateGauge(
        "system_ram_used_bytes",
        "Used system RAM in bytes.");

    public static readonly Gauge SystemRamUsedPercent = Prometheus.Metrics.CreateGauge(
        "system_ram_used_percent",
        "Used system RAM percentage.");

    public static readonly Gauge MysqlUp = Prometheus.Metrics.CreateGauge(
        "mysql_up",
        "MySQL availability. 1 = up, 0 = down.");

    public static readonly Gauge MysqlThreadsConnected = Prometheus.Metrics.CreateGauge(
        "mysql_threads_connected",
        "Current MySQL connected threads.");

    public static readonly Gauge MysqlQuestionsTotal = Prometheus.Metrics.CreateGauge(
        "mysql_questions_total",
        "Total number of statements executed by MySQL.");

    public static readonly Gauge MysqlSlowQueriesTotal = Prometheus.Metrics.CreateGauge(
        "mysql_slow_queries_total",
        "Total number of slow MySQL queries.");

    public static readonly Gauge MysqlUptimeSeconds = Prometheus.Metrics.CreateGauge(
        "mysql_uptime_seconds",
        "MySQL uptime in seconds.");

    public static readonly Gauge SystemCpuUsagePercent = Prometheus.Metrics.CreateGauge(
        "system_cpu_usage_percent",
        "Total system CPU usage percentage.");
}

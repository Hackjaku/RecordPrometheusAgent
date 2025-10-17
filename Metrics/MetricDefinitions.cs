using Prometheus;

namespace PrometheusAgent.Metrics;

public static class MetricDefinitions {
    public static readonly Gauge RamUsage = Prometheus.Metrics.CreateGauge("system_ram_usage_bytes", "Current RAM usage in bytes.");

        // ? Ping metrics
        public static readonly Gauge PingStatus = Prometheus.Metrics.CreateGauge(
            "ping_status",
            "Ping status (1=up, 0=down)",
            new GaugeConfiguration { LabelNames = new[] { "host" } });

        public static readonly Gauge PingLatency = Prometheus.Metrics.CreateGauge(
            "ping_latency_ms",
            "Ping latency in milliseconds",
            new GaugeConfiguration { LabelNames = new[] { "host" } });

        // ? Disk metrics
        public static readonly Gauge DiskTotal = Prometheus.Metrics.CreateGauge(
            "disk_total_bytes",
            "Total disk space in bytes",
            new GaugeConfiguration { LabelNames = new[] { "drive" } });

        public static readonly Gauge DiskFree = Prometheus.Metrics.CreateGauge(
            "disk_free_bytes",
            "Free disk space in bytes",
            new GaugeConfiguration { LabelNames = new[] { "drive" } });
}
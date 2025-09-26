using Prometheus;

namespace PrometheusAgent.Metrics;

public static class MetricDefinitions {
    public static readonly Gauge RamUsage = Prometheus.Metrics.CreateGauge("system_ram_usage_bytes", "Current RAM usage in bytes.");
}
namespace PrometheusAgent.Configuration;

public sealed class AgentConfig {
    public PrometheusConfig Prometheus { get; set; } = new();
    public MonitoringConfig Monitoring { get; set; } = new();
}

public sealed class PrometheusConfig {
    public int Port { get; set; } = 9090;
}

public sealed class MonitoringConfig {
    public PingConfig Ping { get; set; } = new();
    public DiskConfig Disk { get; set; } = new();
    public RamConfig Ram { get; set; } = new();
    public MysqlConfig Mysql { get; set; } = new();
    public ProcessConfig Process { get; set; } = new();
    public CpuConfig Cpu { get; set; } = new();
}

public sealed class PingConfig {
    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 60;
    public List<PingTargetConfig> Targets { get; set; } = new();
}

public sealed class PingTargetConfig {
    public string Name { get; set; } = "";
    public string Host { get; set; } = "";
}

public sealed class DiskConfig {
    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 300;
    public List<string> Drives { get; set; } = new();
}

public sealed class ProcessConfig {
    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 10;
}

public sealed class RamConfig {
    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 10;
}

public sealed class MysqlConfig {
    public bool Enabled { get; set; } = false;
    public int IntervalSeconds { get; set; } = 30;
    public string ConnectionString { get; set; } = "";
}

public sealed class CpuConfig {
    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 5;
}

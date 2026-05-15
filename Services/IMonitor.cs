namespace PrometheusAgent.Services;

public interface IMonitor {
  void Start(CancellationToken cancellationToken);
}

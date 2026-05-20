using MySqlConnector;

using PrometheusAgent.Configuration;
using PrometheusAgent.Metrics;

namespace PrometheusAgent.Services;

public sealed class MysqlMonitor : IMonitor {
    private readonly MysqlConfig _config;

    public MysqlMonitor(MysqlConfig config) {
        _config = config;
    }

    public void Start(CancellationToken cancellationToken) {
        _ = Task.Run(() => RunAsync(cancellationToken), cancellationToken);
    }

    private async Task RunAsync(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            await UpdateMetricsAsync(cancellationToken);

            await Task.Delay(
                TimeSpan.FromSeconds(_config.IntervalSeconds),
                cancellationToken);
        }
    }

    private async Task UpdateMetricsAsync(CancellationToken cancellationToken) {
        try {
            await using var connection = new MySqlConnection(_config.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            var values = await ReadGlobalStatusAsync(connection, cancellationToken);

            MetricDefinitions.MysqlUp.Set(1);

            MetricDefinitions.MysqlThreadsConnected.Set(Get(values, "Threads_connected"));
            MetricDefinitions.MysqlQuestionsTotal.Set(Get(values, "Questions"));
            MetricDefinitions.MysqlSlowQueriesTotal.Set(Get(values, "Slow_queries"));
            MetricDefinitions.MysqlUptimeSeconds.Set(Get(values, "Uptime"));
        }
        catch {
            MetricDefinitions.MysqlUp.Set(0);
        }
    }

    private static async Task<Dictionary<string, double>> ReadGlobalStatusAsync(
        MySqlConnection connection,
        CancellationToken cancellationToken) {
        const string sql = """
            SHOW GLOBAL STATUS
            WHERE Variable_name IN
            (
                'Threads_connected',
                'Questions',
                'Slow_queries',
                'Uptime'
            );
            """;

        var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken)) {
            var name = reader.GetString("Variable_name");
            var rawValue = reader.GetString("Value");

            if (double.TryParse(rawValue, out var value))
                result[name] = value;
        }

        return result;
    }

    private static double Get(Dictionary<string, double> values, string key) {
        return values.TryGetValue(key, out var value)
            ? value
            : 0;
    }
}

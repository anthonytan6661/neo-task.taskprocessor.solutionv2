using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace neo_task.rabbitmq
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly IConfiguration _config;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private bool _disposed;

        public ConnectionProvider(IConfiguration config)
        {
            this._config = config;
            this._factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST")!= null 
                           ? Environment.GetEnvironmentVariable("RABBITMQ_HOST"):  _config["environmentVariables:RABBITMQ_HOST"],
                Port =     Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))!=0
                            ? Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT")): Convert.ToInt32(_config["environmentVariables:RabbitMQ_PORT"]),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER")!=null 
                            ? Environment.GetEnvironmentVariable("RABBITMQ_USER") : _config["environmentVariables:RABBITMQ_USER"],
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!=null 
                            ? Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") : _config["environmentVariables:RABBITMQ_PASSWORD"], 
                VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST")!=null
                                ? Environment.GetEnvironmentVariable("RABBITMQ_VHOST") : _config["environmentVariables:RABBITMQ_VHOST"],
            };
            this._factory.AutomaticRecoveryEnabled = true;
            this._connection = _factory.CreateConnection();
        }

        public IConnection GetConnection()
        {
            return _connection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _connection?.Close();

            _disposed = true;
        }
    }
}

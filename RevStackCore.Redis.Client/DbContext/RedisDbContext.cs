using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RevStackCore.Redis.Client
{
	/// <summary>
	/// StackExchange Redis db context.
	/// </summary>
	public class RedisDbContext
	{
		private const string DEFAULT_CONNECTION = "localhost:6379";
        private const int SYNC_TIMEOUT = 1000;
        private const int CONNECT_RETRY = 3;
        private const int CONNECT_TIMEOUT = 5000;
		private readonly string _connection;
        private readonly bool _abortConnect;
        private readonly int _syncTimeout;
        private readonly int _connectRetry;
        private readonly int _connectTimeout;
        private readonly ConnectionOptions _options;
        private ConnectionMultiplexer _connectionMultiplexer;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:RevStackCore.Redis.RedisDbContext"/> class.
		/// </summary>
		public RedisDbContext()
		{
			_connection = DEFAULT_CONNECTION;
            _abortConnect = false;
            _options = null;
            _syncTimeout = SYNC_TIMEOUT;
            _connectTimeout = CONNECT_TIMEOUT;
            _connectRetry = CONNECT_RETRY;
            setConnectionMultiplexer();
		}

		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:RevStackCore.Redis.RedisDbContext"/> class.
		/// </summary>
		/// <param name="connection">Connection.</param>
		public RedisDbContext(string connection)
		{
			_connection = connection;
            _abortConnect = false;
            _syncTimeout = SYNC_TIMEOUT;
            _connectTimeout = CONNECT_TIMEOUT;
            _connectRetry = CONNECT_RETRY;
            _options = null;
            setConnectionMultiplexer();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RevStackCore.Redis.RedisDbContext"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        public RedisDbContext(string connection, int syncTimeout)
        {
            _connection = connection;
            _abortConnect = false;
            _syncTimeout = syncTimeout;
            _connectTimeout = CONNECT_TIMEOUT;
            _connectRetry = CONNECT_RETRY;
            _options = null;
            setConnectionMultiplexer();
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="T:RevStackCore.Redis.Client.RedisDbContext"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="abortConnect">If set to <c>true</c> abort on connect.</param>
		public RedisDbContext(string connection,bool abortConnect)
		{
			_connection = connection;
            _abortConnect = abortConnect;
            _syncTimeout = SYNC_TIMEOUT;
            _connectTimeout = CONNECT_TIMEOUT;
            _connectRetry = CONNECT_RETRY;
            _options = null;
            setConnectionMultiplexer();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RevStackCore.Redis.Client.RedisDbContext"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="abortConnect">If set to <c>true</c> abort on connect.</param>
        public RedisDbContext(string connection, int syncTimeout, bool abortConnect)
        {
            _connection = connection;
            _abortConnect = abortConnect;
            _syncTimeout = syncTimeout;
            _connectTimeout = CONNECT_TIMEOUT;
            _connectRetry = CONNECT_RETRY;
            _options = null;
            setConnectionMultiplexer();
        }

        public RedisDbContext(string connection, int syncTimeout,int connectTimeout,int connectRetry, bool abortConnect)
        {
            _connection = connection;
            _abortConnect = abortConnect;
            _syncTimeout = syncTimeout;
            _connectTimeout = connectTimeout;
            _connectRetry = connectRetry;
            _options = null;
            setConnectionMultiplexer();
        }

        public RedisDbContext(string connection, ConnectionOptions options)
		{
			_connection = connection;
            _abortConnect = false;
			_options = options;
            setConnectionMultiplexer();
		}

		/// <summary>
		/// Returns an instance of the redis database.
		/// </summary>
		/// <returns>The database.</returns>
		public IDatabase Database()
		{
            return _connectionMultiplexer.GetDatabase();
		}

        /// <summary>
        /// Sets the connection multiplexer.
        /// </summary>
        private void setConnectionMultiplexer()
        {
            if (_options != null)
            {
                ConfigurationOptions options = _options.ToRedisConfigurationOptions();
                options.EndPoints.Add(_connection);
                _connectionMultiplexer = ConnectionMultiplexer.Connect(options);

            }
            else
            {
                string connection = _connection + ",abortConnect=" + _abortConnect.ToString();
                var redisConfig = ConfigurationOptions.Parse(connection);
                redisConfig.SyncTimeout = _syncTimeout;
                redisConfig.ConnectTimeout = _connectTimeout;
                redisConfig.ConnectRetry = _connectRetry;
                _connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfig);
            }
        }
	}
}

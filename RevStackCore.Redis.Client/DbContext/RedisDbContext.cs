using System;
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
		private readonly string _connection;
        private readonly bool _abortConnect;
        private readonly int _syncTimeout;
        private readonly ConnectionOptions _options;
		/// <summary>
		/// Initializes a new instance of the <see cref="T:RevStackCore.Redis.RedisDbContext"/> class.
		/// </summary>
		public RedisDbContext()
		{
			_connection = DEFAULT_CONNECTION;
            _abortConnect = false;
            _options = null;
            _syncTimeout = SYNC_TIMEOUT;
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
            _options = null;
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
            _options = null;
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
            _options = null;
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
            _options = null;
        }


        public RedisDbContext(string connection, ConnectionOptions options)
		{
			_connection = connection;
            _abortConnect = false;
			_options = options;
		}

		/// <summary>
		/// Returns an instance of the redis database.
		/// </summary>
		/// <returns>The database.</returns>
		public IDatabase Database()
		{
            if(_options!=null)
            {
                ConfigurationOptions options = _options.ToRedisConfigurationOptions();
                options.EndPoints.Add(_connection);
				ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
				return redis.GetDatabase();
            }
            else
            {
                string connection = _connection + ",abortConnect=" + _abortConnect.ToString();
                var redisConfig = ConfigurationOptions.Parse(connection);
                redisConfig.SyncTimeout = _syncTimeout;
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConfig);
				return redis.GetDatabase();
            }

		}
	}
}

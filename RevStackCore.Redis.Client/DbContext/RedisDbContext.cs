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
		private readonly string _connection;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:RevStackCore.Redis.RedisDbContext"/> class.
		/// </summary>
		public RedisDbContext()
		{
			_connection = DEFAULT_CONNECTION;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:RevStackCore.Redis.RedisDbContext"/> class.
		/// </summary>
		/// <param name="host">Host.</param>
		/// <param name="port">Port.</param>
		public RedisDbContext(string host, int port)
		{
			_connection = host + ":" + port.ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:RevStackCore.Redis.RedisDbContext"/> class.
		/// </summary>
		/// <param name="connection">Connection.</param>
		public RedisDbContext(string connection)
		{
			_connection = connection;
		}

		/// <summary>
		/// Returns an instance of the redis database.
		/// </summary>
		/// <returns>The database.</returns>
		public IDatabase Database()
		{
			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(_connection);
			return redis.GetDatabase();
		}
	}
}

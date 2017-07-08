using System;
using StackExchange.Redis;

namespace RevStackCore.Redis.Client
{
	public static partial class Extensions
	{
		public static string ToTypedUrnKey(this string key, string type)
		{
			return "urn:" + type.ToLower() + ":" + key;
		}

        public static ConfigurationOptions ToRedisConfigurationOptions(this ConnectionOptions src)
        {
            ConfigurationOptions options = new ConfigurationOptions();
            if(src.AbortOnConnectFail)
            {
                options.AbortOnConnectFail = true;
            }
            if(src.AllowAdmin)
            {
                options.AllowAdmin = true;
            }
            if(!string.IsNullOrEmpty(src.ClientName))
            {
                options.ClientName = src.ClientName;
            }
            if(src.ConnectRetry!=0)
            {
                options.ConnectRetry = src.ConnectRetry;
            }
            if(src.ConnectTimeout!=0)
            {
                options.ConnectTimeout = src.ConnectTimeout;
            }
            if(src.KeepAlive!=0)
            {
                options.KeepAlive = src.KeepAlive;
            }
            if(!string.IsNullOrEmpty(src.Password))
            {
                options.Password = src.Password;
            }
            if(src.ResolveDns)
            {
                options.ResolveDns = true;
            }
            if(src.Ssl)
            {
                options.Ssl = true;
            }
            if(!string.IsNullOrEmpty(src.SslHost))
            {
                options.SslHost = src.SslHost;
            }
            if(src.SyncTimeout!=0)
            {
                options.SyncTimeout = src.SyncTimeout;
            }
            if(!string.IsNullOrEmpty(src.TieBreaker))
            {
                options.TieBreaker = src.TieBreaker;
            }
            if(src.WriteBuffer!=0)
            {
                options.WriteBuffer = src.WriteBuffer;
            }

            return options;
        }
	}
}

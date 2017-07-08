using System;
namespace RevStackCore.Redis.Client
{
    public class ConnectionOptions
    {
        public bool AllowAdmin { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public int ConnectRetry { get; set; }
        public int ConnectTimeout { get; set; }
        public int KeepAlive { get; set; }
        public int SyncTimeout { get; set; }
        public string ClientName { get; set; }
        public string Password { get; set; }
        public bool ResolveDns { get; set; }
        public bool Ssl { get; set; }
        public string SslHost { get; set; }
        public string TieBreaker { get; set; }
        public int WriteBuffer { get; set; }

    }
}

namespace Template_NET_5_WORKER.CoreService.Models.Configuration
{
    using System.Collections.Generic;

    public class MassTransitRabbitConfig
    {
        public MassTransitRabbitConfig()
        {
            this.ClusterNodes = new List<string>();
        }

        public string ClusterName { get; set; }

        public int ClusterPort { get; set; }

        public string VirtualHost { get; set; }

        public IList<string> ClusterNodes { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool UseSSL { get; set; }
    }
}
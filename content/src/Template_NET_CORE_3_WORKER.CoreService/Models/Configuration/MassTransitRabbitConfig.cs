using System.Collections.Generic;

namespace Template_NET_CORE_3_WORKER.CoreService.Models.Configuration
{
    public class MassTransitRabbitConfig
    {
        public MassTransitRabbitConfig()
        {
            this.ClusterNodes = new List<string>();
        }

        public string ClusterName { get; set; }

        public int ClusterPort { get; set; }

        public string VirtualHost { get; set; }

        public List<string> ClusterNodes { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
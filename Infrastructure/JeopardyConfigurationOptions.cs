using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class JeopardyConfigurationOptions
    {
        public ConnectionStringsConfigurationOptions ConnectionStrings { get; set; }
    }

    public class ConnectionStringsConfigurationOptions
    {
        public string MySQL { get; set; }
    }
}

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class JeopardyContextFactory : IDesignTimeDbContextFactory<JeopardyContext>
    {
        public JeopardyContext CreateDbContext(string[] args)
        {
            var connectionString = "server=127.0.0.1;uid=root;database=jeopardy";
            var optionsBuilder = new DbContextOptionsBuilder<JeopardyContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new JeopardyContext(optionsBuilder.Options);
        }
    }
}

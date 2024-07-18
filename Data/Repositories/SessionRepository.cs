using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface ISessionRepository
    {
        Task CreateSession(Session newSession);
        Task<Session> GetSession(Guid idSession);
    }

    public class SessionRepository : ISessionRepository
    {
        private readonly JeopardyContext _jeopardyContext;

        public SessionRepository(JeopardyContext jeopardyContext)
        {
            _jeopardyContext = jeopardyContext;
        }

        public async Task CreateSession(Session newSession)
        {
            await _jeopardyContext.Sessions.AddAsync(newSession).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Session> GetSession(Guid idSession)
        {
            return await _jeopardyContext.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idSession)
                .ConfigureAwait(false);
        }
    }
}

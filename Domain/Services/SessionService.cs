using Data.Models;
using Data.Repositories;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ISessionService
    {
        Task<Session> CreateSession(Session newSession, Professor professor);
        Task<Session> GetSession(Guid idSession);
    }

    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _SessionRepository;
        
        public SessionService(ISessionRepository SessionRepository)
        {
            _SessionRepository = SessionRepository;
        }

        public async Task<Session> CreateSession(Session newSession, Professor professor)
        {
            newSession.Id = Guid.NewGuid();
            newSession.CreatedAt = DateTime.UtcNow;
            newSession.ProfessorId = professor.Id;

            await _SessionRepository.CreateSession(newSession).ConfigureAwait(false);
            return newSession;
        }
        public async Task<Session> GetSession(Guid idSession)
        {
            return await _SessionRepository.GetSession(idSession);
        }
    }
}

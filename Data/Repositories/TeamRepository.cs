using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface ITeamRepository
    {
        Task CreateTeam(Team newTeam);
        Task UpdateTeam(Team updateTeam);
        Task<Team> GetTeamById(Guid id);
        Task<Team> TeamEntity(string name, Guid sessioId);
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly JeopardyContext _jeopardyContext;

        public TeamRepository(JeopardyContext jeopardyContext)
        {
            _jeopardyContext = jeopardyContext;
        }

        public async Task CreateTeam(Team newTeam)
        {
            await _jeopardyContext.Teams.AddAsync(newTeam).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateTeam(Team updateTeam)
        {
            _jeopardyContext.Teams.Update(updateTeam);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Team> GetTeamById(Guid id)
        {
            return await _jeopardyContext.Teams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
        }
        public async Task<Team> TeamEntity(string name, Guid sessioId)
        {
            return await _jeopardyContext.Teams.AsNoTracking().FirstAsync(x => (x.Title == name && x.SessionId == sessioId)).ConfigureAwait(false);
        }
    }
}

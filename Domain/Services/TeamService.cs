using Data;
using Data.Models;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ITeamService
    {
        Task<Team> CreateTeam(Team newTeam);
        Task UpdateTeam(Team updateTeam);
        Task<Team> GetTeamById(Guid id);
        Task<Team> TeamEntity(string name, Guid sessioId);

    }

    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMemberRepository _memberRepository;

        public TeamService(ITeamRepository teamRepository, IMemberRepository memberRepository)
        {
            _teamRepository = teamRepository;
            _memberRepository = memberRepository;   
        }

        public async Task<Team> CreateTeam(Team newTeam)
        {
            newTeam.Id = Guid.NewGuid();
            foreach (var newMember in newTeam.Members)
            {
                newMember.Id = Guid.NewGuid();
            }
            await _teamRepository.CreateTeam(newTeam).ConfigureAwait(false);

           
            return newTeam;

            //var members = newTeam.Members;
            //newTeam.Id= Guid.NewGuid();

            //newTeam.Members = null;
            //await _teamRepository.CreateTeam(newTeam).ConfigureAwait(false);

            //foreach (var newMember in members)
            //{
            //    newMember.Id = Guid.NewGuid();
            //    newMember.TeamId = newTeam.Id;
            //    await _memberRepository.CreateMember(newMember);
            //}
            //return newTeam;
        }
        public async Task UpdateTeam(Team updateTeam)
        {
            await _teamRepository.UpdateTeam(updateTeam).ConfigureAwait(false); 
        }
        public async Task<Team> GetTeamById(Guid id)
        {
            return await _teamRepository.GetTeamById(id).ConfigureAwait(false);
        }
        public async Task<Team> TeamEntity(string name, Guid sessioId)
        {
            return await _teamRepository.TeamEntity(name, sessioId).ConfigureAwait(false); 
        }
    }
}

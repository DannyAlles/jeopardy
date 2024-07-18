using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IProfessorRepository
    {
        Task<Professor> GetProfessorByLogin(string login);
        Task<Professor> GetProfessorById(Guid id);
        Task CreateProfessor(Professor newprofessor);
        Task UpdateProfessor(Professor updateprofessor);
    }

    public class ProfessorRepository : IProfessorRepository
    {
        private readonly JeopardyContext _jeopardyContext;

        public ProfessorRepository(JeopardyContext jeopardyContext)
        {
            _jeopardyContext = jeopardyContext;
        }

        public async Task<Professor> GetProfessorByLogin(string login)
        {
            return await _jeopardyContext.Professors
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Login == login)
                .ConfigureAwait(false);
        }

        public async Task<Professor> GetProfessorById(Guid id)
        {
            return await _jeopardyContext.Professors
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id)
                .ConfigureAwait(false);
        }

        public async Task CreateProfessor(Professor newprofessor)
        {
            await _jeopardyContext.Professors.AddAsync(newprofessor).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateProfessor(Professor updateprofessor)
        {
            _jeopardyContext.Update(updateprofessor);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

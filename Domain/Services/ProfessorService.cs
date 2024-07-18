using Data;
using Data.Models;
using Data.Repositories;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IProfessorService
    {
        Task<Professor> GetById(Guid id);
        Task CreateProfessor(Professor newprofessor);
        Task<Professor> GetProfessorById(Guid id);
        Task<Professor> GetProfessorByLogin(string login);

        Task UpdateProfessor(Professor updateprofessor, string? newpass, string? newpassRepeat);
    }

    public class ProfessorService : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository;

        public ProfessorService(IProfessorRepository professorRepository)
        {
            _professorRepository = professorRepository;
        }

        public async Task<Professor> GetById(Guid id)
        {
            return await _professorRepository.GetProfessorById(id).ConfigureAwait(false);
        }
        public async Task CreateProfessor(Professor newprofessor)
        {
            var professorEntity = await GetProfessorByLogin(newprofessor.Login).ConfigureAwait(false);
            if (professorEntity != null) throw new LoginExistException();

            byte[] salt;
            byte[] buffer2;
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(newprofessor.Password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

            newprofessor.Id = Guid.NewGuid();
            newprofessor.Password = Convert.ToBase64String(dst);
            newprofessor.CreatedAt = DateTime.UtcNow;

            await _professorRepository.CreateProfessor(newprofessor).ConfigureAwait(false);
        }

        public async Task<Professor> GetProfessorById(Guid id)
        {
            return await _professorRepository.GetProfessorById(id).ConfigureAwait(false);
        }

        public async Task<Professor> GetProfessorByLogin(string login)
        {
            return await _professorRepository.GetProfessorByLogin(login).ConfigureAwait(false);
        }

        public async Task UpdateProfessor(Professor updateprofessor, string? newpass, string? newpassRepeat)
        {
            if (!String.IsNullOrEmpty(newpass))
            {
                if (newpass == newpassRepeat)
                {
                    byte[] salt;
                    byte[] buffer2;
                    using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(updateprofessor.Password, 0x10, 0x3e8))
                    {
                        salt = bytes.Salt;
                        buffer2 = bytes.GetBytes(0x20);
                    }
                    byte[] dst = new byte[0x31];
                    Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
                    Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

                    updateprofessor.Password = Convert.ToBase64String(dst);
                }
                else throw new PasswordsNotEqualsException(); 
            }
            await _professorRepository.UpdateProfessor(updateprofessor).ConfigureAwait(false);
        }

    }
}

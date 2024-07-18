using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public interface IMemberRepository
    {
        Task CreateMember(Member newMember);
    }

    public class MemberRepository : IMemberRepository
    {
        private readonly JeopardyContext _jeopardyContext;

        public MemberRepository(JeopardyContext jeopardyContext)
        {
            _jeopardyContext = jeopardyContext;
        }

        public async Task CreateMember(Member newMember)
        {
            await _jeopardyContext.Members.AddAsync(newMember).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        
    }
}


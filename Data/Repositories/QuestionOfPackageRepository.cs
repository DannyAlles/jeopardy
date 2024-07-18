using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public interface IQuestionOfPackageRepository
    {
        Task CreateQuestionOfPackage(QuestionOfPackage newQuestionOfPackage);
        Task<IEnumerable<QuestionOfPackage>> GetQuestionOfPackageList(Guid id);
    }

    public class QuestionOfPackageRepository : IQuestionOfPackageRepository
    {
        private readonly JeopardyContext _jeopardyContext;

        public QuestionOfPackageRepository(JeopardyContext jeopardyContext)
        {
            _jeopardyContext = jeopardyContext;
        }

        public async Task CreateQuestionOfPackage(QuestionOfPackage newQuestionOfPackage)
        {
            await _jeopardyContext.QuestionOfPackages.AddAsync(newQuestionOfPackage).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<QuestionOfPackage>> GetQuestionOfPackageList(Guid id)
        {
            return await _jeopardyContext.QuestionOfPackages.AsNoTracking()
                 .Where(x => x.PackageId == id)
                 .ToListAsync().ConfigureAwait(false);
        }
    }
}
using Data;
using Data.Models;
using Data.Repositories;

namespace Domain.Services
{
    public interface IQuestionOfPackageService
    {
        Task CreateQuestionOfPackage(IEnumerable<QuestionOfPackage> newQuestionOfPackage, Guid idPack);
        Task<IEnumerable<QuestionOfPackage>> GetQuestionOfPackageList(Guid id);
    }

    public class QuestionOfPackageService : IQuestionOfPackageService
    {
        private readonly IQuestionOfPackageRepository _questionOfPackageRepository;

        public QuestionOfPackageService(IQuestionOfPackageRepository questionOfPackageService)
        {
            _questionOfPackageRepository = questionOfPackageService;   
        }

        public async Task CreateQuestionOfPackage(IEnumerable<QuestionOfPackage> newQuestionOfPackage, Guid idPack)
        {
            QuestionOfPackage questionOfPackage = new QuestionOfPackage();
            Guid id = Guid.NewGuid();
            foreach(var question in newQuestionOfPackage)
            {
                questionOfPackage.X = question.X;
                questionOfPackage.Y = question.Y;
                questionOfPackage.QuestionId = question.QuestionId;
                questionOfPackage.PackageId = idPack;
                await _questionOfPackageRepository.CreateQuestionOfPackage(questionOfPackage).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<QuestionOfPackage>> GetQuestionOfPackageList(Guid id)
        {
            return await _questionOfPackageRepository.GetQuestionOfPackageList(id).ConfigureAwait(false);
        }
    }
}

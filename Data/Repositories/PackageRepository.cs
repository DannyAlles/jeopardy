using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IPackageRepository
    {
        Task CreatePackage(Package newPackage);
        Task<Package> GetPackage(Guid id);
        Task<IEnumerable<Package>> GetPackageList(string? title);
        Task<int> GetPackageX(Guid id);
        Task<int> GetPackageY(Guid id);
    }

    public class PackageRepository : IPackageRepository
    {
        private readonly JeopardyContext _jeopardyContext;
        private readonly IQuestionOfPackageRepository _questionOfPackage;

        public PackageRepository(JeopardyContext jeopardyContext, IQuestionOfPackageRepository questionOfPackage)
        {
            _jeopardyContext = jeopardyContext;
            _questionOfPackage = questionOfPackage;
        }

        public async Task CreatePackage(Package newPackage)
        {
            await _jeopardyContext.Packages.AddAsync(newPackage).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Package> GetPackage(Guid id)
        {
            return await _jeopardyContext.Packages.AsNoTracking()
                 .FirstOrDefaultAsync(x => x.Id == id)
                 .ConfigureAwait(false);
        }
        public async Task<IEnumerable<Package>> GetPackageList(string? title)
        {
            return await _jeopardyContext.Packages.AsNoTracking()
                .Where(x => x.Title.ToLower().Contains(title != null ? title.ToLower() : ""))
                .OrderBy(x => x.Title)
                .ToListAsync().ConfigureAwait(false);
        }
        public async Task<int> GetPackageX(Guid id)
        {
            int xMax = -1;
            var packQuestoins = await _questionOfPackage.GetQuestionOfPackageList(id).ConfigureAwait(false);
            if (packQuestoins != null)
            {
                foreach (var question in packQuestoins)
                {
                    if (question.X > xMax) xMax = question.X;
                }
            }
            return xMax;
        }
        public async Task<int> GetPackageY(Guid id)
        {
            int yMax = -1;
            var packQuestoins = await _questionOfPackage.GetQuestionOfPackageList(id).ConfigureAwait(false);
            if (packQuestoins != null)
            {
                foreach (var question in packQuestoins)
                {
                    if (question.Y > yMax) yMax = question.Y;
                }
            }
            return yMax;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Data;
using Data.Models;
using Data.Repositories;
using Domain.Exceptions;

namespace Domain.Services
{
    public interface IPackageService
    {
        Task CreatePackage(Package newPackage, Professor professor);
        Task<Package> GetPackage(Guid id);
        Task<IEnumerable<Package>> GetPackageList(string? title);
        Task<int> GetPackageX(Guid id);
        Task<int> GetPackageY(Guid id);
    }

    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IQuestionOfPackageService _questionOfPackageService;

        public PackageService(IPackageRepository packageRepository, IQuestionOfPackageService questionOfPackageService)
        {
            _packageRepository = packageRepository;
            _questionOfPackageService = questionOfPackageService;
        }

        public async Task CreatePackage(Package newPackage, Professor professor)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            newPackage.CreatedAt = DateTime.UtcNow;
            newPackage.Id = Guid.NewGuid();
            newPackage.CreatedById = professor.Id;
            await _packageRepository.CreatePackage(newPackage).ConfigureAwait(false);
            //await _questionOfPackageService.CreateQuestionOfPackage(newPackage.QuestionOfPackages, newPackage.Id);
            transaction.Complete();
        }

        public async Task<Package> GetPackage(Guid id)
        {
            var package = await _packageRepository.GetPackage(id);
            return package;
        }
        public async Task<IEnumerable<Package>> GetPackageList(string? title)
        {
            var package = await _packageRepository.GetPackageList(title);
            return package;
        }
        public async Task<int> GetPackageX(Guid id)
        {
            return await _packageRepository.GetPackageX(id).ConfigureAwait(false); 
        }
        public async Task<int> GetPackageY(Guid id)
        {
            return await _packageRepository.GetPackageY(id).ConfigureAwait(false);
        }
    }
}

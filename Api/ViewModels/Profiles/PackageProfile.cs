using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models; 

namespace Api.ViewModels.Profiles
{
    public class PackageProfile : Profile
    {
        public PackageProfile()
        {
            CreateMap<CreatePackageViewModel, Package>();
            CreateMap<Package, GetPackageListViewModel>();
        }
    }
}

using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models;

namespace Api.ViewModels.Profiles
{
    public class QuestionOfPackageProfile : Profile
    {
        public QuestionOfPackageProfile()
        {
            CreateMap<CreateQuestionOfPackageViewModel, QuestionOfPackage>();
            CreateMap<QuestionOfPackage, GetQuestionOfPackageListViewModel>();
        }
    }
}

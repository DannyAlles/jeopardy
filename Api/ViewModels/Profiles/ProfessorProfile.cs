using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models;
using Microsoft.Extensions.Logging;

namespace Api.ViewModels.Profiles
{
    public class ProfessorProfile : Profile
    {
        public ProfessorProfile()
        {
            CreateMap<CreateProfessorViewModel, Professor>();
            CreateMap<Professor, GetProfessorViewModel>();
            CreateMap<UpdateProfessorViewModel, Professor>();
        }
    }
}

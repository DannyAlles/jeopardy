using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models;
using Microsoft.Extensions.Logging;

namespace Api.ViewModels.Profiles
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<CreateQuestionViewModel, Question>();
            CreateMap<Question, GetQuestionViewModel>();
            CreateMap<Question, GetQuestionListViewModel>();
            CreateMap<UpdateQuestionViewModel, Question>();
        }
    }
}

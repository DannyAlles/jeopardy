using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models;

namespace Api.ViewModels.Profiles
{
    public class SessionProfile : Profile
    {
        public SessionProfile()
        {
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Session, GetSessionViewModel>();
        }
    }
}

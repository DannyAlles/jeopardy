using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models;

namespace Api.ViewModels.Profiles
{
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<CreateMemberViewModel, Member>();
        }
    }
}

using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models;
using Microsoft.Extensions.Logging;

namespace Api.ViewModels.Profiles
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<CreateTeamViewModel, Team>();
            CreateMap<Team, GetTeamViewModel>();
            CreateMap<UpdateTeamViewModel, Team>();

        }
    }
}

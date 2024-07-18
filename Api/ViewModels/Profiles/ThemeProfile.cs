using Api.ViewModels.Requests;
using Api.ViewModels.Responses;
using AutoMapper;
using Data.Models;
using Microsoft.Extensions.Logging;

namespace Api.ViewModels.Profiles
{
    public class ThemeProfile : Profile
    {
        public ThemeProfile()
        {
            CreateMap<CreateThemeViewModel, Theme>();
            CreateMap<Theme, GetThemeListViewModel>();

        }
    }
}

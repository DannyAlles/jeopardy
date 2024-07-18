using Data;
using Data.Models;
using Data.Repositories;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Services
{
    public interface IThemeService
    {
        Task<Theme> CreateTheme(Theme newTheme);
        Task<Theme> GetThemeByName(string name);
        Task<Theme> GetThemeById(Guid id);
        Task<IEnumerable<Theme>> GetThemeList(string title);

    }

    public class ThemeService : IThemeService
    {
        private readonly IThemeRepository _themeRepository;

        public ThemeService(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }

        public async Task<Theme> CreateTheme(Theme newTheme)
        {
           newTheme.Id = Guid.NewGuid();

           await _themeRepository.CreateTheme(newTheme).ConfigureAwait(false);
            return newTheme;
        }
        public async Task<Theme> GetThemeById(Guid id)
        {
            return await _themeRepository.GetThemeById(id).ConfigureAwait(false);
        }

        public async Task<Theme> GetThemeByName(string name)
        {
            return await _themeRepository.GetThemeByName(name).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Theme>> GetThemeList(string? title)
        {
            return await _themeRepository.GetThemeList(title).ConfigureAwait(false);
        }
    }
}

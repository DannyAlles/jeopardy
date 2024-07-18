using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data.Repositories
{
    public interface IThemeRepository
    {
        Task CreateTheme(Theme newTheme);
        Task<Theme> GetThemeByName(string name);
        Task<Theme> GetThemeById(Guid id);
        Task<IEnumerable<Theme>> GetThemeList(string title);
    }

    public class ThemeRepository : IThemeRepository
    {
        private readonly JeopardyContext _jeopardyContext;

        public ThemeRepository(JeopardyContext jeopardyContext)
        {
            _jeopardyContext = jeopardyContext;
        }

        public async Task CreateTheme(Theme newTheme)
        {
            await _jeopardyContext.Themes.AddAsync(newTheme).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<Theme> GetThemeById(Guid id)
        {
            return await _jeopardyContext.Themes
            .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id)
                .ConfigureAwait(false);
        }
        public async Task<Theme> GetThemeByName(string name)
        {
            return await _jeopardyContext.Themes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Title.Replace(" ", "").ToLower() == name.Replace(" ", "").ToLower())
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Theme>> GetThemeList(string? title)
        {
            return await _jeopardyContext.Themes.AsNoTracking()
                .Where(x => x.Title.ToLower().Contains(title != null ? title.ToLower() : ""))
                .OrderBy(x => x.Title).
                ToListAsync().ConfigureAwait(false);
        }
    }
}

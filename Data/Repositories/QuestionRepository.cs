using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace Data.Repositories
{
    public interface IQuestionRepository
    {
        Task CreateQuestion(Question newQuestion);
        Task<Question> GetQuestion(Guid idQuestion);
        Task<IEnumerable<Question>> GetQuestionList(string? text);
        Task<IEnumerable<Question>> GetQuestionOfThemeList(string? title);
        Task UpdateQuestion(Question updateQuestion);
    }

    public class QuestionRepository : IQuestionRepository
    {
        private readonly JeopardyContext _jeopardyContext;

        public QuestionRepository(JeopardyContext jeopardyContext)
        {
            _jeopardyContext = jeopardyContext;
        }

        public async Task CreateQuestion(Question newQuestion)
        {
            await _jeopardyContext.Questions.AddAsync(newQuestion).ConfigureAwait(false);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Question> GetQuestion(Guid idQuestion)
        {
            return await _jeopardyContext.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == idQuestion)
            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Question>> GetQuestionList(string? text)
        {
            return await _jeopardyContext.Questions
            //.Select(x => new Question()
            // {
            //     Id = x.Id,
            //     QuestionText = x.QuestionText,
            //     Hint = x.Hint,
            //     AnswerText = x.AnswerText,
            //     Theme = new()
            //     {
            //         Id = x.ThemeId,
            //         Title = x.Theme.Title
            //     }
            // })
            .AsNoTracking()
            .Where(x => x.QuestionText.ToLower().Contains(text != null ? text.ToLower() : ""))
            .OrderBy(x => x.CreatedAt)
            .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Question>> GetQuestionOfThemeList(string? title)
        {
            return await _jeopardyContext.Questions
            .AsNoTracking()
            .Where(x => x.Theme.Title.ToLower().Equals(title != null ? title.ToLower() : ""))
            .OrderBy(x => x.CreatedAt)
            .ToListAsync().ConfigureAwait(false);
        }
        public async Task UpdateQuestion(Question updateQuestion)
        {
            _jeopardyContext.Update(updateQuestion);
            await _jeopardyContext.SaveChangesAsync().ConfigureAwait(false);
        }

    }
}

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
using static System.Net.Mime.MediaTypeNames;

namespace Domain.Services
{
    public interface IQuestionService
    {
        Task CreateQuestion(Question newQuestion);
        Task<Question> GetQuestion(Guid idQuestion);
        Task<IEnumerable<Question>> GetQuestionList(string? text);
        Task<IEnumerable<Question>> GetQuestionOfThemeList(string? title);
        Task UpdateQuestion(Question updateQuestion);
    }

    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task CreateQuestion(Question newQuestion)
        {
            newQuestion.Id= Guid.NewGuid();
            newQuestion.CreatedAt = DateTime.UtcNow;

            await _questionRepository.CreateQuestion(newQuestion).ConfigureAwait(false);
        }
        public async Task<Question> GetQuestion(Guid idQuestion)
        {
            return await _questionRepository.GetQuestion(idQuestion);
        }
        public async Task<IEnumerable<Question>> GetQuestionList(string? text)
        {
            return await _questionRepository.GetQuestionList(text).ConfigureAwait(false);

        }

        public async Task<IEnumerable<Question>> GetQuestionOfThemeList(string? title)
        {
            return await _questionRepository.GetQuestionOfThemeList(title).ConfigureAwait(false);

        }

        public async Task UpdateQuestion(Question updateQuestion)
        {
            await _questionRepository.UpdateQuestion(updateQuestion).ConfigureAwait(false);
        }
    }
}

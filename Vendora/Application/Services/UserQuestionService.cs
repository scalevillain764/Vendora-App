using IUserQuestionService = Application.Interfaces.IUserQuestionService;
using Application.DTO.UserQuestionDTO;
using Application.Result;
using Infrastructure.AppDbContexts;
using Domain.ErrorTypes;
using Domain.UserQuestions;
namespace Application.Services
{
    public class UserQuestionService : IUserQuestionService
    {
        private readonly AppDbContext _context;
        public UserQuestionService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<UserQuestionResponseDTO>> AskQuestionAsync(Ulid UserId, Ulid ProductId, UserQuestionCreateAndChangeDTO DTO)
        {
            var product = await _context.Products
                .FindAsync(ProductId);

            if (product == null)
                return Result<UserQuestionResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            var question = new UserQuestion(UserId, ProductId, product.StoreId, DTO.QuestionText, DTO.PhotoUrl);

            _context.UserQuestions.Add(question);

            await _context.SaveChangesAsync();

            return Result<UserQuestionResponseDTO>.Success(new UserQuestionResponseDTO(question, false));
        }
        public async Task<Result<UserQuestionResponseDTO>> RemoveQuestionAsync(Ulid UserId, Ulid QuestionId)
        {
            var question = await _context.UserQuestions
                .FindAsync(QuestionId);

            if (question == null)
                return Result<UserQuestionResponseDTO>.Error("Вопрос не найден", ErrorType.NotFound);

            if (question.UserId != UserId)
                return Result<UserQuestionResponseDTO>.Error("Это не ваш вопрос", ErrorType.Forbidden);

            var dto = new UserQuestionResponseDTO(question, false);

            _context.UserQuestions.Remove(question);

            await _context.SaveChangesAsync();

            return Result<UserQuestionResponseDTO>.Success(dto);
        }
    }
}
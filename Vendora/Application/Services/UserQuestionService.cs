using Application.DTO.ProductReviewDTO;
using Application.DTO.UserQuestionDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.UserQuestions;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using IUserQuestionService = Application.Interfaces.IUserQuestionService;
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

        public async Task<Result<UserQuestionResponseDTO>> ChangeQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuestionCreateAndChangeDTO DTO)
        {
            var question = await _context.UserQuestions
                .FindAsync(QuestionId);

            if (question == null)
                return Result<UserQuestionResponseDTO>.Error("Вопрос не найден", ErrorType.NotFound);

            question.QuestionText = DTO.QuestionText;
            question.PhotoUrls = DTO.PhotoUrl;
            question.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Result<UserQuestionResponseDTO>.Success(new UserQuestionResponseDTO(question, false));
        }

        public async Task<Result<List<UserQuestionResponseDTO>>> GetUserQuestionsToProductAsync(Ulid UserId, Ulid ProductId)
        {
            var product = await _context.Products
                   .Include(x => x.Store)
               .FirstOrDefaultAsync(x => x.Id == ProductId);

            if (product == null)
                return Result<List<UserQuestionResponseDTO>>.Error("Товар не найден", ErrorType.NotFound);

            bool canReply = product.Store.SellerId == ProductId;

            var rez = await _context.UserQuestions
                .Where(x => x.ProductId == ProductId)
                .Select(x => new UserQuestionResponseDTO(x, canReply))
                .ToListAsync();

            return Result<List<UserQuestionResponseDTO>>.Success(rez);
        }

        public async Task<Result<UserQuestionResponseDTO>> ReplyUserQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuastionReplyDTO DTO)
        {
            var question = await _context.UserQuestions
                .Include(x => x.store)
                .FirstOrDefaultAsync(x => x.Id == QuestionId);

            if (question == null)
                return Result<UserQuestionResponseDTO>.Error("Вопрос не найден", ErrorType.NotFound);

            if (question.store.SellerId != UserId)
                return Result<UserQuestionResponseDTO>.Error("Вы не можете ответить на этот вопрос", ErrorType.Forbidden);

            question.SellerReply = DTO.SellerReply;

            await _context.SaveChangesAsync();

            return Result<UserQuestionResponseDTO>.Success(new UserQuestionResponseDTO(question, false));
        }
    }
}
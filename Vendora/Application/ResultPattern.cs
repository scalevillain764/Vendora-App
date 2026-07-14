using Domain.ErrorTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Application.Result
{
    public class Result<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public T? data { get; set; }
        public string? ErrorMessage { get; set; }
        public ErrorType? ErrorType { get; set; }
        public Result(bool success, T? data, string? errorMessage, ErrorType? errorType)
        {
            IsSuccess = success;
            this.data = data;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }
        public static Result<T> Success(T data) => new Result<T>(true, data, null, null);
        public static Result<T> Error(string message, ErrorType type) => new Result<T>(false, null, message, type);
    }
}
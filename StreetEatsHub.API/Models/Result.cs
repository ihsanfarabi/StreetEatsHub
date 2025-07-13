namespace StreetEatsHub.API.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result<T> Success(T data)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Error = error,
                Errors = new List<string> { error }
            };
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Error = errors.FirstOrDefault(),
                Errors = errors
            };
        }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result Success()
        {
            return new Result { IsSuccess = true };
        }

        public static Result Failure(string error)
        {
            return new Result
            {
                IsSuccess = false,
                Error = error,
                Errors = new List<string> { error }
            };
        }

        public static Result Failure(List<string> errors)
        {
            return new Result
            {
                IsSuccess = false,
                Error = errors.FirstOrDefault(),
                Errors = errors
            };
        }
    }
}
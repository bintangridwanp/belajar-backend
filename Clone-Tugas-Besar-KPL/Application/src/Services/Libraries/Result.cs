namespace Tubes_KPL.src.Services.Libraries
{
    // Result class library by zuhri
    public class Result<T>
    {
        public T? Value { get; private set; }
        public string? Error { get; private set; }
        public bool IsSuccess => Error == null;

        public static Result<T> Success(T value)
        {
            if (value is string str)
            {
                return new Result<T> { Value = (T)(object)$"[green]{str}[/]" };
            }
            return new Result<T> { Value = value };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T> { Error = $"[red]{error}[/]" };
        }

        public override string? ToString() => IsSuccess ? Value.ToString() : Error;
    }
}
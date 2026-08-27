namespace LostAndFound.Shared.Dtos;

public class Result
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public static Result Ok() => new() { Succeeded = true, StatusCode = 200 };
    public static Result Fail(string error, int statusCode = 400) =>
        new() { Succeeded = false, Error = error, StatusCode = statusCode };
}

public class Result<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public static Result<T> Ok(T data) => new() { Succeeded = true, Data = data, StatusCode = 200 };
    public static Result<T> Fail(string error, int statusCode = 400) =>
        new() { Succeeded = false, Error = error, StatusCode = statusCode };
}

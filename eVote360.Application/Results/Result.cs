namespace EVote360.Application.Results;

public class Result
{
    public bool Succeeded { get; protected set; }
    public string? Error { get; protected set; }

    public static Result Success() => new() { Succeeded = true };
    public static Result Failure(string error) => new() { Succeeded = false, Error = error };
}

public class Result<T> : Result
{
    public T? Data { get; protected set; }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static Result<T> Failure(string e) => new() { Succeeded = false, Error = e };
}

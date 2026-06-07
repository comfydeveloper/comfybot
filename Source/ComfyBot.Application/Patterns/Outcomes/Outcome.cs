namespace ComfyBot.Application.Patterns.Outcomes;

public class Outcome
{
    public bool IsSuccess { get; init; }
    public Error Error { get; init; }

    public static Outcome Success() => new() { IsSuccess = true };
    public static Outcome Failure(Error error) => new() { IsSuccess = false, Error = error };
}

public class Outcome<T> : Outcome
{
    public T Payload { get; init; }

    public static Outcome<T> Success(T payload) => new() { IsSuccess = true, Payload = payload };
    public static new Outcome<T> Failure(Error error) => new() { IsSuccess = false, Error = error };
}

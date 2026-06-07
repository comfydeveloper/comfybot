namespace ComfyBot.Application.Patterns.Outcomes;

public record NotFoundError : Error
{
    public NotFoundError(string entityType, string id)
        : base($"{entityType} with ID {id} not found.")
    {
    }

    public NotFoundError(string message)
        : base(message)
    {
    }
}

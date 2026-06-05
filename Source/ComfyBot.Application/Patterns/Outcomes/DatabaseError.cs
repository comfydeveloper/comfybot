namespace ComfyBot.Application.Patterns.Outcomes;

public record DatabaseError(string Message) : Error($"Database error: {Message}");

namespace ComfyBot.Application.Patterns.Outcomes;

public record ValidationError(string Message) : Error(Message);

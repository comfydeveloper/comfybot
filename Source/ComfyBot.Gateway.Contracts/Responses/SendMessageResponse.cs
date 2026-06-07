namespace ComfyBot.Gateway.Contracts.Responses;

public class SendMessageResponse
{
    public bool Success { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}

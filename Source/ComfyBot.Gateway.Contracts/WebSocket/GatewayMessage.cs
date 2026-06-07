using System.Text.Json.Serialization;

namespace ComfyBot.Gateway.Contracts.WebSocket;

public class GatewayMessage
{
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("payload")]
    public string Payload { get; set; } = string.Empty;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageType
{
    MessageReceived,
    CommandReceived,
    SendMessage,
    SendMessageResponse,
    Heartbeat,
    Error
}

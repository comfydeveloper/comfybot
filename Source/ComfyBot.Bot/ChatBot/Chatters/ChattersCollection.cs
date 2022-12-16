namespace ComfyBot.Bot.ChatBot.Chatters;

using System.Text.Json.Serialization;

public class ChattersCollection
{
    [JsonPropertyName("chatter_count")]
    public int Count { get; set; }

    [JsonPropertyName("chatters")]
    public Chatters Chatters { get; set; }
}
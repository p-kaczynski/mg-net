using System.Text.Json.Serialization;

namespace mg_net.DataTypes.Sending;

public class MailgunMessageResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message{ get; set; }

    public void Deconstruct(out string id, out string? message)
    {
        id = Id;
        message = Message;
    }
}
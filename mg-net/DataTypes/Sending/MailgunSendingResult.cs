namespace mg_net.DataTypes.Sending;

public class MailgunSendingResult
{
    public MailgunSendingResult(bool success) => Success = success;

    public MailgunSendingResult(bool success, MailgunMessageResponse messageResponse) : this(success) =>
        (Id, Message) = messageResponse;

    public bool Success { get; }

    public string? Id { get; set; }

    public string? Message { get; set; }

    public int StatusCode { get; set; }

    public string? ResponseBody { get; set; }
}

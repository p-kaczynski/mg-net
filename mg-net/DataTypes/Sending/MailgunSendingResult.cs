namespace mg_net.DataTypes.Sending;

public class MailgunSendingResult
{
    public bool Success { get; }

    public MailgunSendingResult(bool success)
    {
        Success = success;
    }

    public MailgunSendingResult(bool success, MailgunMessageResponse messageResponse) : this(success)
    {
        (Id, Message) = messageResponse;
    }

    public string? Id { get; set; }

    public string? Message { get; set; }

    public int StatusCode { get; set; }

    public string? ResponseBody { get; set; }
}
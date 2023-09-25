namespace mg_net.DataTypes.Sending;

public class MailgunSendingResult
{
    private readonly bool _success;

    public MailgunSendingResult(bool success)
    {
        _success = success;
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
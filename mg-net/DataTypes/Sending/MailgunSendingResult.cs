namespace mg_net.DataTypes.Sending;

public class MailgunSendingResult
{
    public MailgunSendingResult()
    {
    }

    public MailgunSendingResult(MailgunMessageResponse messageResponse)
        => (Id, Message) = messageResponse;

    public string? Id { get; set; }

    public string? Message { get; set; }
    
    public int StatusCode { get; set; }
    
    public string? ResponseBody { get; set; }
}
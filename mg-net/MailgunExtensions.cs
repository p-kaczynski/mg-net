namespace mg_net;

internal static class MailgunExtensions
{
    public static string ToMailgunDateString(this DateTimeOffset dto)
    {
        const string format = "ddd, dd MMM yyyy HH:mm:ss";
        const string suffix = " +0000";

        return dto.ToUniversalTime().ToString(format) + suffix;
    }
}

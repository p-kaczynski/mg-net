using Microsoft.Extensions.DependencyInjection;

namespace mg_net.Extensions.AspNet;

public static class MailgunClientAspNetExtensions
{
    public static void AddMailgunClient(this IServiceCollection services, Action<MailgunClientOptions> optionAction,
        Action<IServiceProvider, HttpClient> clientAction)
    {
        services.Configure(optionAction);
        services.AddHttpClient<MailgunClient>(clientAction);
        services.AddScoped<MailgunClient>();
    }
    
    public static void AddMailgunClient(this IServiceCollection services, Action<MailgunClientOptions> optionAction,
        Uri mailgunBaseUri)
        => AddMailgunClient(services, optionAction, (_, client) => { client.BaseAddress = mailgunBaseUri; });

    public static void AddMailgunClient(this IServiceCollection services, Action<MailgunClientOptions> optionAction,
        string mailgunBaseUri = "https://api.mailgun.net/")
        => AddMailgunClient(services, optionAction, new Uri(mailgunBaseUri, UriKind.Absolute));
}
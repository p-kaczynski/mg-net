using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace mg_net.Extensions.AspNet;

public static class MailgunClientAspNetExtensions
{
    public static void AddMailgunClient(this IServiceCollection services, Action<OptionsBuilder<MailgunClientOptions>> optionAction,
        Action<IServiceProvider, HttpClient> clientAction)
    {
        optionAction.Invoke(services.AddOptions<MailgunClientOptions>());
        services.AddHttpClient<MailgunClient>(clientAction);
        services.AddScoped<MailgunClient>();
    }

    public static void AddMailgunClient(this IServiceCollection services, Action<OptionsBuilder<MailgunClientOptions>> optionAction,
        Uri mailgunBaseUri)
        => AddMailgunClient(services, optionAction, (_, client) => { client.BaseAddress = mailgunBaseUri; });

    public static void AddMailgunClient(this IServiceCollection services, Action<OptionsBuilder<MailgunClientOptions>> optionAction,
        string mailgunBaseUri = "https://api.mailgun.net/")
        => AddMailgunClient(services, optionAction, new Uri(mailgunBaseUri, UriKind.Absolute));
}
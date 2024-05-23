using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using JetBrains.Annotations;
using mg_net.DataTypes.Sending;
using mg_net.DataTypes.Webhooks;
using Microsoft.Extensions.Options;

namespace mg_net;

public class MailgunClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<MailgunClientOptions> _options;

    public MailgunClient(HttpClient httpClient, IOptions<MailgunClientOptions> options)
    {
        _httpClient = httpClient;
        if (_httpClient.BaseAddress is null)
        {
            throw new($"{nameof(HttpClient)} for {nameof(MailgunClientOptions)} has no base address");
        }

        _options = options;
    }

    [PublicAPI]
    public async Task<bool> TrySetWebhook(WebhookEvent.Type webhookType, Uri webhookUri,
        CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();


        var request = new HttpRequestMessage(HttpMethod.Post,
            string.Format(MailgunApiEndpoints.Webhooks, _options.Value.Domain));

        request.Headers.Authorization = new("Basic", GetAuthString());
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("id", WebhookEvent.ByType[webhookType]),
            new KeyValuePair<string, string>("url", webhookUri.ToString())
        });

        var response = await _httpClient.SendAsync(request, cancellationToken.Value).ConfigureAwait(false);
        Trace.WriteLine($"TrySetWebhook: ({response.StatusCode}):\n{await response.Content.ReadAsStringAsync()}");
        return response.IsSuccessStatusCode;
    }

    [PublicAPI]
    public async Task<bool> TryRemoveWebhook(WebhookEvent.Type webhookType, CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();

        var request = new HttpRequestMessage(HttpMethod.Delete,
            $"{string.Format(MailgunApiEndpoints.Webhooks, _options.Value.Domain)}/{WebhookEvent.ByType[webhookType]}");

        request.Headers.Authorization = new("Basic", GetAuthString());

        var response = await _httpClient.SendAsync(request, cancellationToken.Value).ConfigureAwait(false);
        Trace.WriteLine($"TryRemoveWebhook: ({response.StatusCode}):\n{await response.Content.ReadAsStringAsync()}");
        return response.IsSuccessStatusCode;
    }

    [PublicAPI]
    public async Task<MailgunSendingResult> Send(MailgunMessage message, CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();

        var request = new HttpRequestMessage(HttpMethod.Post,
            string.Format(MailgunApiEndpoints.Messages, _options.Value.Domain));

        request.Headers.Authorization = new("Basic", GetAuthString());
        request.Content = message.ToHttpContent();

        var response = await _httpClient.SendAsync(request, cancellationToken.Value).ConfigureAwait(false);

        var result =
            response.IsSuccessStatusCode
            && await response.Content
                    .ReadFromJsonAsync<MailgunMessageResponse>(cancellationToken: cancellationToken.Value)
                    .ConfigureAwait(false) is
            { } messageResponse
                ? new MailgunSendingResult(true, messageResponse)
                : new(false);


        result.StatusCode = (int)response.StatusCode;
        result.ResponseBody = await response.Content.ReadAsStringAsync(
#if !NETSTANDARD2_1
            cancellationToken.Value
#endif
        ).ConfigureAwait(false);

        return result;
    }

    private string GetAuthString() => Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{_options.Value.ApiKey}"));
}

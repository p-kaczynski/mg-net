using System.Net.Http.Json;
using System.Text;
using mg_net.DataTypes.Sending;
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
            throw new($"{nameof(HttpClient)} for {nameof(MailgunClientOptions)} has no base address");
        _options = options;
    }

    public async Task<MailgunSendingResult> Send(MailgunMessage message, CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

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
        result.ResponseBody = await response.Content.ReadAsStringAsync(cancellationToken.Value).ConfigureAwait(false);

        return result;
    }

    private string GetAuthString() => Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{_options.Value.ApiKey}"));
}
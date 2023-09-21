using System.Net.Mail;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace mg_net.DataTypes.Sending;

public class MailgunMessage
{
    [PropertyName("from")] public required MailAddress From { get; set; }
    [PropertyName("to")] public IList<MailAddress>? To { get; set; }
    [PropertyName("cc")] public IList<MailAddress>? Cc { get; set; }
    [PropertyName("bcc")] public IList<MailAddress>? Bcc { get; set; }
    [PropertyName("subject")] public required string Subject { get; set; }
    [PropertyName("text")] public string? Text { get; set; }
    [PropertyName("html")] public string? Html { get; set; }
    [PropertyName("amp-html")] public string? AmpHtml { get; set; }

    [PropertyName("attachment")]
    public IDictionary<string, byte[]> Attachment { get; init; } = new Dictionary<string, byte[]>();

    [PropertyName("inline")]
    public IDictionary<string, byte[]> Inline { get; init; } = new Dictionary<string, byte[]>();

    [PropertyName("template")] public string? Template { get; set; }
    [PropertyName("t:version")] public string? TemplateVersion { get; set; }
    [PropertyName("t:text")] private MailgunBool TemplateToText { get; set; } = null;
    [PropertyName("t:variables")] public JsonObject? TemplateVariables { get; set; }
    [PropertyName("o:tag")] public IList<string> Tag { get; init; } = new List<string>();
    [PropertyName("o:dkim")] public MailgunBool Dkim { get; set; } = null;
    [PropertyName("o:deliverytime")] public DateTimeOffset? DeliveryTime { get; set; }

    [PropertyName("o:deliverytime-optimize-period")]
    public MailgunHours? DeliveryTimeOptimizePeriod { get; set; }

    [PropertyName("o:time-zone-localize")] public MailgunTzoString? TimeZoneLocalize { get; set; }
    [PropertyName("o:testmode")] public bool TestMode { get; set; }
    [PropertyName("o:tracking")] public MailgunBool Tracking { get; set; } = null;
    [PropertyName("o:tracking-clicks")] public MailgunBool TrackingClicks { get; set; } = null;
    [PropertyName("o:tracking-opens")] public MailgunBool TrackingOpens { get; set; } = null;

    [PropertyName("o:tracking-pixel-location-top")]
    public MailgunBool TrackingPixelLocationTop { get; set; } = null;

    [PropertyName("o:require-tls")] public MailgunBool RequireTls { get; set; } = null;
    [PropertyName("o:skip-verification")] public MailgunBool SkipVerification { get; set; }

    [PropertyName("recipient-variables")] public JsonObject? RecipientVariables { get; set; }

    public IDictionary<string, string>? CustomHeaders { get; set; }
    public IDictionary<string, string>? CustomVars { get; set; }

    public HttpContent ToHttpContent() => ConvertToContentData(this);

    #region HttpContent conversion

    private static readonly Dictionary<string, string> PropertiesMapping = typeof(MailgunMessage)
        .GetProperties()
        .Select(p => p.GetCustomAttribute<PropertyNameAttribute>() is { } attr ? (p.Name, attr.Name) : (null, null))
        .Where(x => x is not (null, null))
        .ToDictionary(x => x.Item1!, x => x.Item2!);

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static MultipartFormDataContent ConvertToContentData(MailgunMessage m)
    {
        var content = new MultipartFormDataContent();

        AddStringContent(nameof(From), m.From.ToString());
        AddStringContent(nameof(Subject), m.Subject);

        if (m.To is {Count: > 0} to)
            AddStringContents(nameof(To), to.Select(t => t.ToString()));

        if (m.Cc is {Count: > 0} cc)
            AddStringContents(nameof(Cc), cc.Select(t => t.ToString()));

        if (m.Bcc is {Count: > 0} bcc)
            AddStringContents(nameof(Bcc), bcc.Select(t => t.ToString()));

        AddStringContent(nameof(Text), m.Text);
        AddStringContent(nameof(Html), m.Html);
        AddStringContent(nameof(AmpHtml), m.AmpHtml);

        AddAttachments(nameof(Attachment), m.Attachment);
        AddAttachments(nameof(Inline), m.Inline);

        AddStringContent(nameof(Template), m.Template);
        AddStringContent(nameof(TemplateVersion), m.TemplateVersion);
        AddMailgunBool(nameof(TemplateToText), m.TemplateToText);
        AddJsonDictionary(nameof(TemplateVariables), m.TemplateVariables);

        AddStringContents(nameof(Tag), m.Tag);
        AddMailgunBool(nameof(Dkim), m.Dkim);

        AddDateTimeOffset(nameof(DeliveryTime), m.DeliveryTime);
        AddMailgunHours(nameof(DeliveryTimeOptimizePeriod), m.DeliveryTimeOptimizePeriod);
        AddMailgunTzoString(nameof(TimeZoneLocalize), m.TimeZoneLocalize);

        AddMailgunBool(nameof(Tracking), m.Tracking);
        AddMailgunBool(nameof(TrackingClicks), m.TrackingClicks);
        AddMailgunBool(nameof(TrackingOpens), m.TrackingOpens);
        AddMailgunBool(nameof(TrackingPixelLocationTop), m.TrackingPixelLocationTop);
        AddMailgunBool(nameof(RequireTls), m.RequireTls);
        AddMailgunBool(nameof(SkipVerification), m.SkipVerification);

        AddJsonDictionary(nameof(RecipientVariables), m.RecipientVariables);

        AddArbitrary('h', m.CustomHeaders);
        AddArbitrary('v', m.CustomVars);

        if (m.TestMode)
            AddStringContent(nameof(TestMode), true.ToString());

        return content;

        void AddStringContents(string name, IEnumerable<string>? values)
        {
            if (values is not null)
                foreach (var value in values)
                    AddStringContent(name, value);
        }

        void AddStringContent(string name, string? value)
        {
            if (value is not null)
                content?.Add(new StringContent(value), PropertiesMapping[name]);
        }

        void AddAttachments(string name, IDictionary<string, byte[]> values)
        {
            foreach (var value in values)
                content.Add(new ByteArrayContent(value.Value), PropertiesMapping[name], value.Key);
        }

        void AddMailgunBool(string name, MailgunBool mb)
        {
            if (mb.GetValue() is { } value)
                AddStringContent(name, value);
        }

        void AddJsonDictionary(string name, JsonObject? jObj)
        {
            if (jObj is not null && JsonSerializer.Serialize(jObj, SerializerOptions) is {Length: > 0} json)
                AddStringContent(name, json);
        }

        void AddDateTimeOffset(string name, DateTimeOffset? dto)
        {
            if (dto.HasValue)
                AddStringContent(name, dto.Value.ToMailgunDateString());
        }

        void AddMailgunHours(string name, MailgunHours? hours)
        {
            if (hours.HasValue)
                AddStringContent(name, hours.Value.GetValue());
        }

        void AddMailgunTzoString(string name, MailgunTzoString? str)
        {
            if (str.HasValue)
                AddStringContent(name, str.Value.GetValue());
        }

        void AddArbitrary(char prefix, IDictionary<string, string>? maybeDictionary)
        {
            if (maybeDictionary is {Count: > 0})
                foreach (var (key, value) in maybeDictionary)
                    content.Add(new StringContent(value), $"{prefix}:{key}");
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    private class PropertyNameAttribute : Attribute
    {
        public PropertyNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    #endregion
}
namespace mg_net.DataTypes.Webhooks;

public static class WebhookEvent
{
    public enum Type
    {
        Accepted,
        Clicked,
        Complained,
        Delivered,
        Opened,
        PermanentFail,
        TemporaryFail,
        Unsubscribed
    }

    public static readonly IReadOnlyDictionary<string, Type> ByName =
        new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["accepted"] = Type.Accepted,
            ["clicked"] = Type.Clicked,
            ["complained"] = Type.Complained,
            ["delivered"] = Type.Delivered,
            ["opened"] = Type.Opened,
            ["permanent_fail"] = Type.PermanentFail,
            ["temporary_fail"] = Type.TemporaryFail,
            ["unsubscribed"] = Type.Unsubscribed
        };

    public static readonly IReadOnlyDictionary<Type, string> ByType =
        ByName.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
}

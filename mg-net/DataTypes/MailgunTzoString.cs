namespace mg_net.DataTypes;

public readonly struct MailgunTzoString
{
    private readonly TimeSpan _ts;

    public MailgunTzoString(int hours, int minutes) => _ts = new(0, hours, minutes, 0);

    public MailgunTzoString(TimeSpan offset) => _ts = offset;

    public string GetValue() => $"{_ts.Hours}:{_ts.Minutes}";
}

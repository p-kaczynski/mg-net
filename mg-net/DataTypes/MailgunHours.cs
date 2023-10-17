namespace mg_net.DataTypes;

public readonly struct MailgunHours
{
    private readonly int _h;

    private MailgunHours(int h)
    {
        if (h <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(h), $"{nameof(h)} must be positive integer");
        }

        _h = h;
    }

    public static implicit operator MailgunHours(int h) => new(h);

    public string GetValue() => $"{_h}h";
}

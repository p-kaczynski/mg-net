namespace mg_net.DataTypes;

public readonly struct MailgunBool
{
    private readonly bool? _b;

    private MailgunBool(bool? b) => _b = b;

    public string? GetValue()
        => _b switch
        {
            true => "yes",
            false => "no",
            _ => null
        };

    public static implicit operator MailgunBool(bool b) => new(b);
    public static implicit operator MailgunBool(bool? b) => new(b);
    public static implicit operator bool(MailgunBool mb) => mb._b is true;
}

using System.Net.Mail;

namespace EVote360.Domain.ValueObjects;

public sealed class EmailAddress : IEquatable<EmailAddress>
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("El email no puede estar vacío.", nameof(input));

        try
        {
            var addr = new MailAddress(input.Trim());
            return new EmailAddress(addr.Address);
        }
        catch
        {
            throw new ArgumentException("Email inválido.", nameof(input));
        }
    }

    public override string ToString() => Value;

    public bool Equals(EmailAddress? other) => other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is EmailAddress other && Equals(other);
    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

    public static bool operator ==(EmailAddress left, EmailAddress right) => left.Equals(right);
    public static bool operator !=(EmailAddress left, EmailAddress right) => !left.Equals(right);
}


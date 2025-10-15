using System.Text.RegularExpressions;

namespace EVote360.Domain.ValueObjects;


///cedula  normalizada a 11 digitos (sin guiones)

public sealed class NationalId : IEquatable<NationalId>
{
    public string Value { get; }

    private NationalId(string value) => Value = value;

    public static NationalId Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("La cédula no puede estar vacía.", nameof(input));

        // Elimina no-digitos y normaliza a 11
        var digits = Regex.Replace(input, "[^0-9]", "");
        if (digits.Length != 11)
            throw new ArgumentException("La cédula debe tener 11 dígitos.", nameof(input));

        return new NationalId(digits);
    }

    public override string ToString() => Value;

    public bool Equals(NationalId? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is NationalId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public static bool operator ==(NationalId left, NationalId right) => left.Equals(right);
    public static bool operator !=(NationalId left, NationalId right) => !left.Equals(right);
}


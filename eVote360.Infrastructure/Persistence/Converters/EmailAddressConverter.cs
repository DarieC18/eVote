using EVote360.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EVote360.Infrastructure.Persistence.Converters;

public class EmailAddressConverter : ValueConverter<EmailAddress, string>
{
    public EmailAddressConverter() : base(
        v => v.Value,
        v => EmailAddress.Create(v))
    { }
}

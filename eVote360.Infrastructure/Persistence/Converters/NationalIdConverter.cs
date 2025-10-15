using EVote360.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EVote360.Infrastructure.Persistence.Converters;

public class NationalIdConverter : ValueConverter<NationalId, string>
{
    public NationalIdConverter() : base(
        v => v.Value,
        v => NationalId.Create(v))
    { }
}

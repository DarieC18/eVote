using EVote360.Application.Common.Ocr;

namespace EVote360.Infrastructure.Ocr;
public class TesseractOcrMock : IOcrService
{
    public Task<string> ExtractNationalIdAsync(Stream frontImage, CancellationToken ct = default)
    {
        return Task.FromResult("00112345678");
    }
}

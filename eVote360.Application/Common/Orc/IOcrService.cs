namespace EVote360.Application.Common.Ocr;
public interface IOcrService
{
    // Devuelve el texto extraido (mockeado) de la imagen del documento
    Task<string> ExtractNationalIdAsync(Stream frontImage, CancellationToken ct = default);
}

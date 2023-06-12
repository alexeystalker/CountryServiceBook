namespace CountryWiki.DAL.Compression;

public class BrotliCompressionProvider : ICompressionProvider
{
    private readonly CompressionLevel? _compressionLevel;

    public BrotliCompressionProvider(CompressionLevel compressionLevel)
    {
        _compressionLevel = compressionLevel;
    }

    public BrotliCompressionProvider() { }

    public Stream CreateCompressionStream(Stream stream, CompressionLevel? compressionLevel)
    {
        var currentCompressionLevel = compressionLevel ?? _compressionLevel ?? CompressionLevel.Fastest;

        return new BrotliStream(stream, currentCompressionLevel, true);
    }

    public Stream CreateDecompressionStream(Stream stream)
    {
        return new BrotliStream(stream, CompressionMode.Decompress);
    }

    public string EncodingName => "br";
}
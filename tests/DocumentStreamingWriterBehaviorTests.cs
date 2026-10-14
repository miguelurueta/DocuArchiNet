using System;
using System.IO;
using System.Linq;

internal static class DocumentStreamingWriterBehaviorTests
{
    public static void Run()
    {
        var bytes = Enumerable.Range(0, EscritorStreamingDocumento.TamanoBuffer * 2 + 31).Select(i => (byte)(i % 251)).ToArray();
        using (var source = new MemoryStream(bytes))
        using (var destination = new MemoryStream())
        {
            var written = EscritorStreamingDocumento.Copiar(source, destination, 123, bytes.Length - 321);
            var expected = bytes.Skip(123).Take(bytes.Length - 321).ToArray();
            Assert(written == expected.Length, "Cantidad escrita exacta.");
            Assert(destination.ToArray().SequenceEqual(expected), "Contenido parcial exacto.");
        }

        using (var source = new MemoryStream(bytes))
        using (var destination = new MemoryStream())
        {
            var checks = 0;
            var written = EscritorStreamingDocumento.Copiar(source, destination, 0, bytes.Length, () => ++checks < 2);
            Assert(written == EscritorStreamingDocumento.TamanoBuffer, "La desconexión detiene después de un buffer.");
        }

        using (var source = new MemoryStream(bytes))
        using (var destination = new ThrowingStream())
        {
            var failed = false;
            try { EscritorStreamingDocumento.Copiar(source, destination, 0, bytes.Length); }
            catch (IOException) { failed = true; }
            Assert(failed, "Propaga una falla de salida para que el handler la sanee.");
            Assert(source.CanRead, "El escritor no corrompe el stream de origen; el propietario puede cerrarlo.");
        }
    }

    private sealed class ThrowingStream : MemoryStream
    {
        public override void Write(byte[] buffer, int offset, int count) { throw new IOException("synthetic path secret"); }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

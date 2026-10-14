using System;
using System.IO;
using System.Text;

internal static class DocumentStreamingStrategyBehaviorTests
{
    public static void Run()
    {
        Assert(!EstrategiaCargaPdf.EsCabeceraLinearizada(Encoding.ASCII.GetBytes("%PDF-1.7\n1 0 obj << /Type /Catalog >>")),
            "Un PDF común no se clasifica como linearizado.");
        Assert(EstrategiaCargaPdf.EsCabeceraLinearizada(Encoding.ASCII.GetBytes("%PDF-1.7\n1 0 obj << /Linearized 1 /L 2000000 >>")),
            "Reconoce el diccionario de linearización inicial.");

        var directory = Path.Combine(Path.GetTempPath(), "doc102-strategy-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var small = Path.Combine(directory, "small.pdf");
            WritePdf(small, true, EstrategiaCargaPdf.UmbralRangosBytes - 1);
            Assert(EstrategiaCargaPdf.Seleccionar(small) == EstrategiaCargaPdf.ModoStreamingCompleto,
                "Un PDF pequeño evita el costo de múltiples rangos.");

            var ordinary = Path.Combine(directory, "ordinary.pdf");
            WritePdf(ordinary, false, EstrategiaCargaPdf.UmbralRangosBytes + 1);
            Assert(EstrategiaCargaPdf.Seleccionar(ordinary) == EstrategiaCargaPdf.ModoStreamingCompleto,
                "Un PDF no linearizado usa streaming secuencial.");

            var linearized = Path.Combine(directory, "linearized.pdf");
            WritePdf(linearized, true, EstrategiaCargaPdf.UmbralRangosBytes + 1);
            Assert(EstrategiaCargaPdf.Seleccionar(linearized) == EstrategiaCargaPdf.ModoRangos,
                "Un PDF grande y linearizado usa rangos.");
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    private static void WritePdf(string path, bool linearized, long length)
    {
        var header = Encoding.ASCII.GetBytes(linearized
            ? "%PDF-1.7\n1 0 obj << /Linearized 1 /L 2000000 >>\n"
            : "%PDF-1.7\n1 0 obj << /Type /Catalog >>\n");
        using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            stream.Write(header, 0, header.Length);
            stream.SetLength(length);
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

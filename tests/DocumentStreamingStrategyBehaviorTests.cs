using System;
using System.IO;
using System.Text;

internal static class DocumentStreamingStrategyBehaviorTests
{
    public static void Run()
    {
        Assert(EstrategiaCargaPdf.EsCabeceraPdf(Encoding.ASCII.GetBytes("%PDF-1.7\n1 0 obj << /Type /Catalog >>")),
            "Reconoce una cabecera PDF válida sin exigir linearización.");
        Assert(!EstrategiaCargaPdf.EsCabeceraPdf(Encoding.ASCII.GetBytes("contenido no PDF")),
            "Rechaza una cabecera que no pertenece a un PDF.");
        Assert(!EstrategiaCargaPdf.EsCabeceraLinearizada(Encoding.ASCII.GetBytes("%PDF-1.7\n1 0 obj << /Type /Catalog >>")),
            "Un PDF común no se clasifica como linearizado.");
        Assert(EstrategiaCargaPdf.EsCabeceraLinearizada(Encoding.ASCII.GetBytes("%PDF-1.7\n1 0 obj << /Linearized 1 /L 2000000 >>")),
            "Reconoce el diccionario de linearización inicial.");
        long declaredLength = 0;
        long firstSectionEnd = 0;
        Assert(EstrategiaCargaPdf.TryObtenerSeccionInicialLinearizada(
                Encoding.ASCII.GetBytes("%PDF-1.7\n1 0 obj << /Linearized 1 /L 2000000 /E 250000 >>"),
                ref declaredLength, ref firstSectionEnd) && declaredLength == 2000000 && firstSectionEnd == 250000,
            "Obtiene la longitud y el fin de la primera sección del diccionario linealizado.");
        Assert(!EstrategiaCargaPdf.TryObtenerSeccionInicialLinearizada(
                Encoding.ASCII.GetBytes("%PDF-1.7\n1 0 obj << /Linearized 1 /L 2000000 >>"),
                ref declaredLength, ref firstSectionEnd),
            "No acepta como eficaz una cabecera sin límite de primera sección.");

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
                "Un PDF grande no linearizado conserva streaming secuencial.");
            Assert(EstrategiaCargaPdf.EsCandidatoDerivado(ordinary),
                "Un PDF grande no linearizado puede preparar un derivado sin tocar el original.");

            var linearized = Path.Combine(directory, "linearized.pdf");
            WritePdf(linearized, true, EstrategiaCargaPdf.UmbralRangosBytes + 1);
            Assert(EstrategiaCargaPdf.Seleccionar(linearized) == EstrategiaCargaPdf.ModoRangos,
                "Un PDF grande y linearizado usa rangos.");
            Assert(!EstrategiaCargaPdf.EsCandidatoDerivado(linearized),
                "Un PDF ya linearizado no genera una copia redundante.");

            var invalid = Path.Combine(directory, "invalid.pdf");
            File.WriteAllText(invalid, "contenido no PDF");
            using (var stream = new FileStream(invalid, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                stream.SetLength(EstrategiaCargaPdf.UmbralRangosBytes + 1);
            }
            Assert(EstrategiaCargaPdf.Seleccionar(invalid) == EstrategiaCargaPdf.ModoStreamingCompleto,
                "Una cabecera inválida conserva el fallback de streaming.");
            Assert(!EstrategiaCargaPdf.EsCandidatoDerivado(invalid),
                "Una cabecera inválida no ejecuta el linealizador.");
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

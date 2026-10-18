using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

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

    public static void RunCacheCleanup()
    {
        // Only synthetic files under this unique, test-owned directory are touched.
        var directory = Path.Combine(Path.GetTempPath(), "doc105-cache-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var foreign = CacheFixture(directory, "foreign.pdf", null, true);
            var unmarked = CacheFixture(directory, new string('a', 64) + ".pdf", null, true);
            var legacy = CacheFixture(directory, new string('b', 64) + ".pdf", "", true);
            var wrong = CacheFixture(directory, new string('c', 64) + ".pdf", "unknown", true);
            var expiredName = new string('d', 64) + ".pdf";
            var expired = CacheFixture(directory, expiredName, "DocuArchiNet.PdfCache.v1:" + expiredName, true);
            var previewName = new string('e', 64) + ".first.pdf";
            var preview = CacheFixture(directory, previewName, "DocuArchiNet.PdfCache.v1:" + previewName, true);
            var recentName = new string('f', 64) + ".pdf";
            var recent = CacheFixture(directory, recentName, "DocuArchiNet.PdfCache.v1:" + recentName, false);
            var temporary = CacheFixture(directory, new string('1', 64) + ".operation.tmp.pdf", "", true);
            var arbitrary = CacheFixture(directory, "other.pdf", "DocuArchiNet.PdfCache.v1:other.pdf", true);
            Directory.CreateDirectory(Path.Combine(directory, "nested"));
            var nested = CacheFixture(Path.Combine(directory, "nested"), expiredName, "DocuArchiNet.PdfCache.v1:" + expiredName, true);
            typeof(PreparadorVisualizacionPdf).GetMethod("LimpiarExpirados", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { directory });
            foreach (var preserved in new[] { foreign, unmarked, legacy, wrong, recent, temporary, arbitrary, nested })
                Assert(File.Exists(preserved) && File.ReadAllText(preserved) == "%PDF-synthetic-only",
                    "Cleanup must preserve foreign, unowned, recent and nested files: " + Path.GetFileName(preserved));
            Assert(!File.Exists(expired) && !File.Exists(expired + ".use"), "Owned expired derivative and marker removed.");
            Assert(!File.Exists(preview) && !File.Exists(preview + ".use"), "Owned expired preview and marker removed.");

            var access = typeof(PreparadorVisualizacionPdf).GetMethod("RegistrarAcceso", BindingFlags.NonPublic | BindingFlags.Static);
            access.Invoke(null, new object[] { unmarked, false });
            access.Invoke(null, new object[] { legacy, false });
            Assert(!File.Exists(unmarked + ".use") && File.ReadAllText(legacy + ".use") == "",
                "Access must not adopt unmarked or legacy files.");
            var publishedName = new string('2', 64) + ".pdf";
            var published = CacheFixture(directory, publishedName, null, true);
            access.Invoke(null, new object[] { published, true });
            Assert(File.ReadAllText(published + ".use") == "DocuArchiNet.PdfCache.v1:" + publishedName,
                "Publication writes a versioned ownership marker.");
            access.Invoke(null, new object[] { wrong, true });
            Assert(File.ReadAllText(wrong + ".use") == "unknown", "Publication never overwrites a foreign marker.");

            var activeName = new string('3', 64) + ".first.pdf";
            var active = CacheFixture(directory, activeName, "DocuArchiNet.PdfCache.v1:" + activeName, true);
            var activeMap = (ConcurrentDictionary<string, byte>)typeof(PreparadorVisualizacionPdf)
                .GetField("EnPreparacion", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            activeMap.TryAdd(new string('3', 64), 0);
            try
            {
                typeof(PreparadorVisualizacionPdf).GetMethod("LimpiarExpirados", BindingFlags.NonPublic | BindingFlags.Static)
                    .Invoke(null, new object[] { directory });
                Assert(File.Exists(active), "Active generation is preserved even with an expired marker.");
            }
            finally { byte removed; activeMap.TryRemove(new string('3', 64), out removed); }

            var lockedName = new string('4', 64) + ".pdf";
            var locked = CacheFixture(directory, lockedName, "DocuArchiNet.PdfCache.v1:" + lockedName, true);
            using (var held = new FileStream(locked + ".use", FileMode.Open, FileAccess.Read, FileShare.None))
                typeof(PreparadorVisualizacionPdf).GetMethod("LimpiarExpirados", BindingFlags.NonPublic | BindingFlags.Static)
                    .Invoke(null, new object[] { directory });
            Assert(File.Exists(locked), "Unreadable ownership marker fails closed without deleting the PDF.");
            Assert(!File.Exists(active), "Inactive owned expired preview can be cleaned on the next pass.");
            Assert(File.Exists(published), "Fresh publication is retained.");
        }
        finally { Directory.Delete(directory, true); }
    }

    private static string CacheFixture(string directory, string name, string marker, bool old)
    {
        var path = Path.Combine(directory, name);
        File.WriteAllText(path, "%PDF-synthetic-only");
        File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddDays(-10));
        if (marker != null)
        {
            File.WriteAllText(path + ".use", marker);
            File.SetLastWriteTimeUtc(path + ".use", old ? DateTime.UtcNow.AddDays(-10) : DateTime.UtcNow);
        }
        return path;
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

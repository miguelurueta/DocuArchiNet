using System;
using System.IO;

internal static class DocumentStreamingReferenceBehaviorTests
{
    public static void Run()
    {
        var directory = Path.Combine(Path.GetTempPath(), "doc102-ref-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var file = Path.Combine(directory, "private-document.pdf");
        File.WriteAllBytes(file, new byte[] { 1, 2, 3, 4 });
        try
        {
            var codec = new DocumentoStreamingReferencia();
            var token = codec.Emitir(file, "document.pdf", "application/pdf", "session-a", DateTime.UtcNow.AddMinutes(2));
            Assert(!string.IsNullOrWhiteSpace(token), "Debe emitir una referencia.");
            Assert(!token.Contains(file) && !token.Contains("document.pdf"), "La referencia no expone ruta o nombre.");

            DocumentoStreamingAutorizado resolved = null;
            Assert(codec.TryResolver(token, "session-a", DateTime.UtcNow, ref resolved), "La misma sesión debe resolver.");
            Assert(string.Equals(Path.GetFullPath(file), resolved.Ruta, StringComparison.OrdinalIgnoreCase), "Debe recuperar la ruta canónica.");
            Assert(!codec.TryResolver(token, "session-b", DateTime.UtcNow, ref resolved), "Otra sesión debe fallar cerrada.");
            Assert(!codec.TryResolver(token + "x", "session-a", DateTime.UtcNow, ref resolved), "Un token alterado debe fallar.");
            Assert(!codec.TryResolver(token, "session-a", DateTime.UtcNow.AddMinutes(3), ref resolved), "Un token expirado debe fallar.");
            Assert(string.IsNullOrEmpty(codec.Emitir(Path.Combine(directory, "sub", "..", "private-document.pdf"), "x.pdf", "application/pdf", "session-a")), "No debe emitir traversal textual.");

            var missing = codec.Emitir(Path.Combine(directory, "missing.pdf"), "missing.pdf", "application/pdf", "session-a", DateTime.UtcNow.AddMinutes(2));
            Assert(!codec.TryResolver(missing, "session-a", DateTime.UtcNow, ref resolved), "Un archivo inexistente no se abre.");
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

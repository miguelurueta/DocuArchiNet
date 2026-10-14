using System;

internal static class DocumentStreamingConditionalBehaviorTests
{
    public static void Run()
    {
        var modified = new DateTime(2026, 10, 13, 20, 30, 15, 987, DateTimeKind.Utc);
        var normalized = CondicionesHttpDocumento.NormalizarHttpDate(modified);
        var etag = CondicionesHttpDocumento.CrearETag("opaque-reference", 4096, modified);
        Assert(etag.StartsWith("\"") && etag.EndsWith("\"") && !etag.Contains("opaque-reference"), "ETag fuerte y opaco.");
        Assert(CondicionesHttpDocumento.EsNoModificado(etag, null, etag, modified), "ETag coincidente devuelve 304.");
        Assert(CondicionesHttpDocumento.EsNoModificado("W/" + etag, null, etag, modified), "If-None-Match permite comparación débil.");
        Assert(!CondicionesHttpDocumento.EsNoModificado("\"other\"", normalized.ToString("R"), etag, modified), "If-None-Match no coincidente tiene precedencia.");
        Assert(CondicionesHttpDocumento.EsNoModificado(null, normalized.ToString("R"), etag, modified), "Fecha vigente devuelve 304.");
        Assert(!CondicionesHttpDocumento.EsNoModificado(null, normalized.AddSeconds(-1).ToString("R"), etag, modified), "Fecha anterior devuelve 200.");
        Assert(CondicionesHttpDocumento.IfRangePermiteRango(etag, etag, modified), "If-Range fuerte conserva 206.");
        Assert(!CondicionesHttpDocumento.IfRangePermiteRango("W/" + etag, etag, modified), "If-Range débil degrada a 200.");
        Assert(CondicionesHttpDocumento.IfRangePermiteRango(normalized.ToString("R"), etag, modified), "Fecha vigente conserva 206.");
        Assert(!CondicionesHttpDocumento.IfRangePermiteRango(normalized.AddSeconds(-1).ToString("R"), etag, modified), "Fecha anterior degrada a 200.");
        Assert(!CondicionesHttpDocumento.IfRangePermiteRango("invalid", etag, modified), "Fecha inválida degrada a 200.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

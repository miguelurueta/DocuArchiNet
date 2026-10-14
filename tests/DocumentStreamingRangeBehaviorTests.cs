using System;

internal static class DocumentStreamingRangeBehaviorTests
{
    public static void Run()
    {
        Valid("bytes=0-99", 1000, 0, 99);
        Valid("bytes=100-199", 1000, 100, 199);
        Valid("bytes=900-", 1000, 900, 999);
        Valid("bytes=-100", 1000, 900, 999);
        Valid("bytes=999-999", 1000, 999, 999);
        Valid("bytes=900-1200", 1000, 900, 999);
        Valid("bytes=-1200", 1000, 0, 999);
        Invalid("items=0-1", 1000);
        Invalid("bytes=0-1,4-5", 1000);
        Invalid("bytes=-0", 1000);
        Invalid("bytes=1000-", 1000);
        Invalid("bytes=20-10", 1000);
        Invalid("bytes=0-0", 0);

        IntervaloBytesDocumento interval = null;
        Assert(RangoHttpDocumento.Resolver(null, 1000, ref interval) == ResultadoRangoHttp.SinRango, "Sin cabecera no hay rango.");
    }

    private static void Valid(string value, long total, long start, long end)
    {
        IntervaloBytesDocumento interval = null;
        Assert(RangoHttpDocumento.Resolver(value, total, ref interval) == ResultadoRangoHttp.Valido, value);
        Assert(interval.Inicio == start && interval.Fin == end && interval.Longitud == end - start + 1, value + " límites");
    }

    private static void Invalid(string value, long total)
    {
        IntervaloBytesDocumento interval = null;
        Assert(RangoHttpDocumento.Resolver(value, total, ref interval) == ResultadoRangoHttp.Invalido && interval == null, value);
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

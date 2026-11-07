// Synthetic .NET Framework process only. No application assembly, IIS, network,
// database, credentials or business files. Ground truth is inside these methods.
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web.Script.Serialization;
public static class Doc117Alm03TimingProbe {
    private static readonly ConcurrentQueue<object> Samples = new ConcurrentQueue<object>();
    private static void Sample(string flow, string phase, string mode) {
        Samples.Enqueue(new { flow, phase, mode, ticks = Stopwatch.GetTimestamp().ToString(), threadId = Thread.CurrentThread.ManagedThreadId.ToString() });
    }
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ImportWriter(string mode) {
        Sample("IMPORTACION", "ENTER", mode);
        Thread.Sleep(500);
        Sample("IMPORTACION", "EXIT", mode);
    }
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void IndividualWriter(string mode) {
        Sample("INDIVIDUAL", "ENTER", mode);
        Thread.Sleep(500);
        Sample("INDIVIDUAL", "EXIT", mode);
    }
    public static int Main(string[] args) {
        if (args.Length != 1 || File.Exists(args[0])) return 2;
        using (var start = new ManualResetEvent(false)) {
            var first = new Thread(() => { start.WaitOne(); ImportWriter("OVERLAPPED"); });
            var second = new Thread(() => { start.WaitOne(); IndividualWriter("OVERLAPPED"); });
            first.Start(); second.Start(); start.Set(); first.Join(); second.Join();
        }
        ImportWriter("SERIALIZED"); IndividualWriter("SERIALIZED");
        var payload = new { syntheticOnly = true, frequencyHz = Stopwatch.Frequency.ToString(), highResolution = Stopwatch.IsHighResolution, samples = Samples.ToArray() };
        using (var stream = new FileStream(args[0], FileMode.CreateNew, FileAccess.Write, FileShare.None))
        using (var writer = new StreamWriter(stream)) {
            writer.Write(new JavaScriptSerializer().Serialize(payload)); writer.Flush(); stream.Flush(true);
        }
        return 0;
    }
}

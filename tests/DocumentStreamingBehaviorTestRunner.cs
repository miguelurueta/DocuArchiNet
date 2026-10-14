using System;

internal static class DocumentStreamingBehaviorTestRunner
{
    private static int Main()
    {
        DocumentStreamingReferenceBehaviorTests.Run();
        DocumentStreamingRangeBehaviorTests.Run();
        DocumentStreamingConditionalBehaviorTests.Run();
        DocumentStreamingWriterBehaviorTests.Run();
        DocumentStreamingStrategyBehaviorTests.Run();
        Console.WriteLine("document streaming behavior tests: passed");
        return 0;
    }
}

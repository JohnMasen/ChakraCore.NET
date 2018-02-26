namespace ChakraCore.NET.DebugAdapter.VSCode
{
    public enum DebugAdapterStatusEnum
    {
        WaitingForEngineReady,
        Ready,
        BreakPointHit,
        StepComplete,
        AsyncBreakHit,
        ExceptionOccured
    }
}
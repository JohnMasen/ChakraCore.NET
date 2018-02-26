namespace ChakraCore.NET.DebugAdapter.VSCode
{
    public class OnStatusChangArguments
    {
        public DebugAdapterStatusEnum Previous { get; private set; }
        public DebugAdapterStatusEnum Current { get; private set; }
        public OnStatusChangArguments(DebugAdapterStatusEnum previous, DebugAdapterStatusEnum current)
        {
            Previous = previous;
            Current = current;
        }
        public override string ToString()
        {
            return $"Previous={Previous}, Current={Current}";
        }
    }
}
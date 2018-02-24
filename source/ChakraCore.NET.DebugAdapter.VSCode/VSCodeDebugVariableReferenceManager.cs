using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.DebugAdapter.VSCode
{
    internal enum VariableScopeEnum
    {
        Local,
        Globals,
        Scopes
    }
    internal struct VariableHandle
    {
        public int Id;
        public uint FrameId;
        public VariableScopeEnum Scope;
    }
    class VSCodeDebugVariableReferenceManager
    {
        private int id;
        private Dictionary<int, VariableHandle> handles;

        public VSCodeDebugVariableReferenceManager()
        {
            Reset();
        }
        public void Reset()
        {
            id = 0;
            handles = new Dictionary<int, VariableHandle>();
        }
        public VariableHandle Create(uint frameId, VariableScopeEnum scope)
        {
            VariableHandle result = new VariableHandle() { FrameId = frameId, Scope = scope, Id =int.MaxValue-id++ };
            handles.Add(result.Id,result);
            return result;
        }

        public bool TryGet(int id, out VariableHandle result)
        {
            return handles.TryGetValue(id, out result);
        }
    }
}

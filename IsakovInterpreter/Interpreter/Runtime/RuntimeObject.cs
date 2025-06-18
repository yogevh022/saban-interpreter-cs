#nullable enable
using System.Collections.Generic;

namespace IsakovInterpreter.Interpreter.Runtime;

public class RuntimeObject : BaseRuntimeNode
{
    public readonly Dictionary<RuntimePrimitive, BaseRuntimeNode> Properties = new();
    
    public bool TryGetValue(BaseRuntimeNode key, out BaseRuntimeNode? value)
    {
        value = null;
        if (key is not RuntimePrimitive primitiveKey)
            return false;
        return Properties.TryGetValue(primitiveKey, out value);
    }

    public override string ToString()
    {
        return "{ " + string.Join(", ", Properties.Select(kv => $"{kv.Key}: {kv.Value}")) + " }";
    }
}
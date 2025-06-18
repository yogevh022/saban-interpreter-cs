#nullable enable
using System;
using System.Collections.Generic;

namespace IsakovInterpreter.Interpreter.Runtime;

public class RuntimeList : BaseRuntimeNode
{
    public List<BaseRuntimeNode> Elements = new();
    
    public bool TryGetAt(BaseRuntimeNode index, out BaseRuntimeNode? element)
    {
        element = null;
        if (index is RuntimeInt intIndexNode)
        {
            var primitiveIndex = (int)intIndexNode.Value;
            if (Elements.Count >= primitiveIndex && primitiveIndex >= 0)
            {
                element = Elements[primitiveIndex];
                return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return "[ " + string.Join(", ", Elements) + " ]";
    }
}
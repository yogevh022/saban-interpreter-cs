#nullable enable
using System.Collections.Generic;
using IsakovInterpreter.Interpreter.Runtime;

namespace IsakovInterpreter.Interpreter.Environment;

public class Environment
{
    private readonly List<RuntimeObject> _scopeChain = new()
    {
        new RuntimeObject(), // env scope
        new RuntimeObject()  // global scope
    };

    public RuntimeObject GetScope()
    {
        return _scopeChain[^1];
    }

    public void AscendScope()
    {
        _scopeChain.Add(new RuntimeObject());
    }

    public void DescendScope()
    {
        if (_scopeChain.Count <= 2)
            throw new System.InvalidOperationException("Cannot descend scope, already at the global scope.");
        _scopeChain.RemoveAt(_scopeChain.Count - 1);
    }

    public BaseRuntimeNode? TryGetFromScope(BaseRuntimeNode key)
    {
        for (int i = _scopeChain.Count - 1; i >= 1; i--) // skipping env scope
        {
            if (_scopeChain[i].TryGetValue(key, out var value))
                return value;
        }

        return null;
    }
}
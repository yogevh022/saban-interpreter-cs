using System;
using System.Collections.Generic;
using IsakovInterpreter.Parser.Types;

namespace IsakovInterpreter.Interpreter;

public class Interpreter
{
    private BaseTypeNode[] _ast;
    private Dictionary<string, BaseTypeNode> _memory;
    private Dictionary<Type, Func<BaseTypeNode, object>> _typeInterpretationHandlers;
    
    public Interpreter(BaseTypeNode[] ast)
    {
        _ast = ast;
        _memory = new Dictionary<string, BaseTypeNode>();
        
    }

    private object IdentityValue(IdentifierNode identifier)
    {
        var currentValue = _memory;
        foreach (var part in identifier.Address)
        {
            object primitivePart = InterpretType(part);
        }
        return new object(); // temp
    }

    private object ExecuteAssignment(AssignmentNode assignment)
    {
        return new object(); // temp
    }

    private object ExecuteBinaryOperation(BinaryOperationNode operation)
    {
        return new object(); // temp
    }

    private object InterpretType(BaseTypeNode node)
    {
        if (_typeInterpretationHandlers.TryGetValue(node.GetType(), out var handler))
            return handler(node);
        throw new NotSupportedException($"Type {node.GetType()} is not supported for interpretation.");
    }
}
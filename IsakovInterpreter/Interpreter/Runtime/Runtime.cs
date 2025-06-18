using IsakovInterpreter.Lexer;
using IsakovInterpreter.Parser.Types;

namespace IsakovInterpreter.Interpreter.Runtime;

public class Runtime
{
    private readonly Environment.Environment _env = new();
    private readonly Dictionary<Type, Func<BaseTypeNode, BaseRuntimeNode>> _typeRuntimeResolutionHandlers;
    private readonly Dictionary<Type, Func<BaseRuntimeNode, BaseRuntimeNode, BaseRuntimeNode>> _memberAccessHandlers;
    private readonly Dictionary<Type, Action<BaseRuntimeNode, BaseRuntimeNode, BaseRuntimeNode>> _memberAssignmentHandlers;

    public Runtime()
    {
        _typeRuntimeResolutionHandlers = new()
        {
            { typeof(IntNode), ResolveRuntimeInt },
            { typeof(DoubleNode), ResolveRuntimeDouble },
            { typeof(StringNode), ResolveRuntimeString },
            { typeof(IdentifierNode), ResolveRuntimeIdentity },
            { typeof(AssignmentNode), ResolveRuntimeAssignment },
            { typeof(ListNode), ResolveRuntimeList },
            { typeof(ObjectNode), ResolveRuntimeObject },
            { typeof(BinaryOperationNode), ResolveBinaryOperation },
            // { typeof(FunctionCallNode), node => ((FunctionCallNode)node).Arguments }
        };
        _memberAccessHandlers = new()
        {
            { typeof(RuntimeObject), ResolveObjectKey },
            { typeof(RuntimeList), ResolveListIndex }
        };
        _memberAssignmentHandlers = new()
        {
            { typeof(RuntimeObject), AssignToObject },
            { typeof(RuntimeList), AssignToList }
        };
    }

    private BaseRuntimeNode ResolveRuntimeNode(BaseTypeNode node)
    {
        if (_typeRuntimeResolutionHandlers.TryGetValue(node.GetType(), out var handler))
            return handler(node);
        
        throw new NotSupportedException($"Type {node.GetType()} is not supported at runtime.");
    }

    private RuntimeInt ResolveRuntimeInt(BaseTypeNode intNode)
    {
        return new RuntimeInt(((IntNode)intNode).Value);
    }
    
    private RuntimeDouble ResolveRuntimeDouble(BaseTypeNode doubleNode)
    {
        return new RuntimeDouble(((DoubleNode)doubleNode).Value);
    }
    
    private RuntimeString ResolveRuntimeString(BaseTypeNode stringNode)
    {
        return new RuntimeString(((StringNode)stringNode).Value);
    }
    
    private BaseRuntimeNode ResolveRuntimeIdentity(BaseTypeNode identifierNode)
    {
        return MemoryCursor(((IdentifierNode)identifierNode).Address.ToArray());
    }

    private BaseRuntimeNode ResolveObjectKey(BaseRuntimeNode obj, BaseRuntimeNode key)
    {
        if (((RuntimeObject)obj).TryGetValue(key, out var value))
            return value!;
        throw new KeyNotFoundException($"Cannot access Object property '{key.GetType()}'.");
    }

    private BaseRuntimeNode ResolveListIndex(BaseRuntimeNode list, BaseRuntimeNode index)
    {
        if (((RuntimeList)list).TryGetAt(index, out var element))
            return element!;
        throw new IndexOutOfRangeException($"Index {index.GetType()} is invalid.");
    }

    private void AssignToObject(BaseRuntimeNode objectNode, BaseRuntimeNode key, BaseRuntimeNode valueNode)
    {
        if (key is not RuntimePrimitive primitiveKey)
            throw new NotSupportedException($"Object Key assignment for type {key.GetType()} is not supported.");
        ((RuntimeObject)objectNode).Properties[primitiveKey] = valueNode;
    }
    
    private void AssignToList(BaseRuntimeNode listNode, BaseRuntimeNode indexNode, BaseRuntimeNode valueNode)
    {
        var list = (RuntimeList)listNode;
        if (list.TryGetAt(indexNode, out _))
            list.Elements[((RuntimePrimitive)indexNode).Value.GetHashCode()] = valueNode;
        throw new IndexOutOfRangeException($"Index {((RuntimePrimitive)indexNode).Value} is invalid for List.");
    }
    
    private BaseRuntimeNode MemoryCursor(BaseTypeNode[] address)
    {
        if (address.Length == 0) // identifier address is empty, return current scope
            return _env.GetScope();
        
        var runtimeKeyNode = ResolveRuntimeNode(address[0]);
        var identifierHead = _env.TryGetFromScope(runtimeKeyNode);
        if (identifierHead == null)
            throw new KeyNotFoundException($"Identifier '{runtimeKeyNode.GetType()}' not found in the current scope.");
        for (int i = 1; i < address.Length; i++)
        {
            runtimeKeyNode = ResolveRuntimeNode(address[0]);
            if (_memberAccessHandlers.TryGetValue(identifierHead.GetType(), out var handler))
                identifierHead = handler(identifierHead, runtimeKeyNode);
            else
                throw new NotSupportedException($"Member access for type {identifierHead.GetType()} is not supported.");
        }

        return identifierHead;
    }
    
    private BaseRuntimeNode ResolveRuntimeAssignment(BaseTypeNode assignmentNode)
    {
        var assignment = (AssignmentNode)assignmentNode;
        var valueAddress = assignment.Identifier.Address;
        var targetContainer = MemoryCursor(valueAddress[..^1].ToArray());
        var lastKey = ResolveRuntimeNode(valueAddress[^1]);
        BaseRuntimeNode? returnValue = null;
        
        if (assignment.ReturnBeforeAssignment && 
            _memberAccessHandlers.TryGetValue(targetContainer.GetType(), out var memberAccessHandler))
            returnValue = memberAccessHandler(targetContainer, lastKey);
        
        if (!_memberAssignmentHandlers.TryGetValue(targetContainer.GetType(), out var memberAssignmentHandler))
            throw new NotSupportedException($"Member assignment for type {targetContainer.GetType()} is not supported.");
        
        var newValue = ResolveRuntimeNode(assignment.Value);
        memberAssignmentHandler(targetContainer, lastKey, newValue);
        return returnValue ?? newValue;
    }
    
    private RuntimeList ResolveRuntimeList(BaseTypeNode listNode)
    {
        var runtimeList = new RuntimeList();
        foreach (var element in ((ListNode)listNode).Elements)
        {
            runtimeList.Elements.Add(ResolveRuntimeNode(element));
        }
        return runtimeList;
    }

    private RuntimeObject ResolveRuntimeObject(BaseTypeNode objectNode)
    {
        var runtimeObject = new RuntimeObject();
        foreach (var objectProp in ((ObjectNode)objectNode).Properties)
        {
            runtimeObject.Properties[(RuntimePrimitive)ResolveRuntimeNode(objectProp.Key)] = ResolveRuntimeNode(objectProp.Value);
        }

        return runtimeObject;
    }
    
    private RuntimePrimitive ResolveBinaryOperation(BaseTypeNode node)
    {
        var binaryOperationNode = (BinaryOperationNode)node;
        var runtimeLeft = ResolveRuntimeNode(binaryOperationNode.Left);
        var runtimeRight = ResolveRuntimeNode(binaryOperationNode.Right);
        var returnValue = (RuntimeNumber)runtimeLeft;
        switch (binaryOperationNode.Operator)
        {
            case TokenType.Plus:
                returnValue += (RuntimeInt)runtimeRight;
                break;
            case TokenType.Minus:
                returnValue -= (RuntimeInt)runtimeRight;
                break;
            case TokenType.Star:
                returnValue *= (RuntimeInt)runtimeRight;
                break;
            case TokenType.Slash:
                returnValue /= (RuntimeInt)runtimeRight;
                break;
            case TokenType.Modulo:
                returnValue %= (RuntimeInt)runtimeRight;
                break;
            case TokenType.Exponent:
                returnValue = returnValue.Pow((RuntimeNumber)runtimeRight);
                break;
            default:
                throw new NotSupportedException($"Operator {binaryOperationNode.Operator} is not supported for type {runtimeLeft.GetType()} and {runtimeRight.GetType()}.");
        }
        return returnValue;
    }
    
    public BaseRuntimeNode Execute(BaseTypeNode node)
    {
        return ResolveRuntimeNode(node);
    }
}
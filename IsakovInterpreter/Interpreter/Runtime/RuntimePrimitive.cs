namespace IsakovInterpreter.Interpreter.Runtime;

public abstract class RuntimePrimitive : BaseRuntimeNode
{
    public abstract object Value { get; set; }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is RuntimePrimitive other && Value.Equals(other.Value);
    }
    
    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }
}
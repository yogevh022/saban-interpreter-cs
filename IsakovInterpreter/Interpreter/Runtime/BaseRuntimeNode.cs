namespace IsakovInterpreter.Interpreter.Runtime;

public class BaseRuntimeNode
{
    public override string ToString()
    {
        return GetType().Name;
    }
}
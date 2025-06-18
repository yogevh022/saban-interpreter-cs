namespace IsakovInterpreter.Interpreter.Runtime;

public class RuntimeInt : RuntimeNumber
{
    private int _value;

    public override object Value
    {
        get => _value;
        set
        {
            if (value is int intValue)
                _value = intValue;
            else
                throw new InvalidCastException($"Cannot assign value of type {value.GetType()} to RuntimeInt.");
        }
    }
    
    public RuntimeInt(int value)
    {
        _value = value;
    }
}
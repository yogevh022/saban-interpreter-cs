namespace IsakovInterpreter.Interpreter.Runtime;

public class RuntimeString : RuntimePrimitive
{
    private string _value;

    public override object Value
    {
        get => _value;
        set
        {
            if (value is string stringValue)
                _value = stringValue;
            else
                throw new InvalidCastException($"Cannot assign value of type {value.GetType()} to RuntimeString.");
        }
    }
    
    public RuntimeString(string value)
    {
        _value = value;
    }
    
    public override string ToString()
    {
        return "'" + _value + "'";
    }
}
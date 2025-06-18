using System.Globalization;

namespace IsakovInterpreter.Interpreter.Runtime;

public class RuntimeDouble : RuntimeNumber
{
    private double _value;

    public override object Value
    {
        get => _value;
        set
        {
            if (value is double doubleValue)
                _value = doubleValue;
            else
                throw new InvalidCastException($"Cannot assign value of type {value.GetType()} to RuntimeDouble.");
        }
    }
    
    public RuntimeDouble(double value)
    {
        _value = value;
    }
    
    public override string ToString()
    {
        return _value.ToString(CultureInfo.InvariantCulture);
    }
}
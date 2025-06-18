namespace IsakovInterpreter.Interpreter.Runtime;

public abstract class RuntimeNumber : RuntimePrimitive
{
    public static RuntimeNumber operator +(RuntimeNumber left, RuntimeNumber right)
    {
        if (left is RuntimeDouble || right is RuntimeDouble)
            return new RuntimeDouble(Convert.ToDouble(left.Value) + Convert.ToDouble(right.Value));
        return new RuntimeInt((int)left.Value + (int)right.Value);
    }
    
    public static RuntimeNumber operator -(RuntimeNumber left, RuntimeNumber right)
    {
        if (left is RuntimeDouble || right is RuntimeDouble)
            return new RuntimeDouble(Convert.ToDouble(left.Value) - Convert.ToDouble(right.Value));
        return new RuntimeInt((int)left.Value - (int)right.Value);
    }
    
    public static RuntimeNumber operator *(RuntimeNumber left, RuntimeNumber right)
    {
        if (left is RuntimeDouble || right is RuntimeDouble)
            return new RuntimeDouble(Convert.ToDouble(left.Value) * Convert.ToDouble(right.Value));
        return new RuntimeInt((int)left.Value * (int)right.Value);
    }
    
    public static RuntimeNumber operator /(RuntimeNumber left, RuntimeNumber right)
    {
        if (Convert.ToDouble(right.Value) == 0)
            throw new DivideByZeroException("Division by zero is not allowed.");
        if (left is RuntimeDouble || right is RuntimeDouble)
            return new RuntimeDouble(Convert.ToDouble(left.Value) / Convert.ToDouble(right.Value));
        return new  RuntimeInt((int)left.Value / (int)right.Value);
    }
    
    public static RuntimeNumber operator %(RuntimeNumber left, RuntimeNumber right)
    {
        if (Convert.ToDouble(right.Value) == 0)
            throw new DivideByZeroException("Division by zero is not allowed.");
        if (left is RuntimeDouble || right is RuntimeDouble)
            return new  RuntimeDouble(Convert.ToDouble(left.Value) % Convert.ToDouble(right.Value));
        return new RuntimeInt((int)left.Value % (int)right.Value);
    }

    public RuntimeNumber Pow(RuntimeNumber exponent)
    {
        if (this is RuntimeDouble || exponent is RuntimeDouble || Convert.ToDouble(exponent.Value) < 0)
            return new RuntimeDouble(Math.Pow(Convert.ToDouble(Value), Convert.ToDouble(exponent.Value)));
        return new RuntimeInt((int)Math.Pow((int)Value, (int)exponent.Value));
    }
}
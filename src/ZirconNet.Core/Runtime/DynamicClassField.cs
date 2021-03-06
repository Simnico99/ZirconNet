namespace ZirconNet.Core.Runtime;
public class DynamicClassField
{
    public DynamicClassField(string name, Type type, object? value = null)
    {
        FieldName = name;
        FieldType = type;
        Value = value;
    }

    public string FieldName { get; }

    public Type FieldType { get; }

    public object? Value { get; }
}
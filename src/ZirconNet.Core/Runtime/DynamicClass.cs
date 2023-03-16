using System.Dynamic;

namespace ZirconNet.Core.Runtime;
public sealed class DynamicClass : DynamicObject
{
    private readonly Dictionary<string, (Type Type, object? Value)> _fields;

    public DynamicClass(IEnumerable<DynamicClassField> fields)
    {
        _fields = new Dictionary<string, (Type Type, object? Value)>();
        foreach (var field in fields)
        {
            _fields.Add(field.FieldName, (field.FieldType, field.Value));
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (_fields.TryGetValue(binder.Name, out var fieldInfo))
        {
            if (value?.GetType() == fieldInfo.Type || value == null)
            {
                _fields[binder.Name] = (fieldInfo.Type, value);
                return true;
            }
            throw new ArgumentException($"{value} type ({value?.GetType()}) is not the same as the Field ({fieldInfo.Type.Name})", nameof(value));
        }
        return false;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_fields.TryGetValue(binder.Name, out var fieldInfo))
        {
            result = fieldInfo.Value;
            return true;
        }
        result = null;
        return false;
    }
}
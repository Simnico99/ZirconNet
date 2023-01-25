using System.Dynamic;

namespace ZirconNet.Core.Runtime;
public sealed class DynamicClass : DynamicObject
{
    private readonly Dictionary<string, KeyValuePair<Type, object?>> _fields;

    public DynamicClass(IEnumerable<DynamicClassField> fields)
    {
        _fields = new Dictionary<string, KeyValuePair<Type, object?>>();
        foreach (var field in fields)
        {
            _fields.Add(field.FieldName, new KeyValuePair<Type, object?>(field.FieldType, field.Value));
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (!_fields.ContainsKey(binder.Name))
        {
            return false;
        }
        var type = _fields[binder.Name].Key;
        if (value?.GetType() == type)
        {
            _fields[binder.Name] = new KeyValuePair<Type, object?>(type, value);
            return true;
        }
        throw new ArgumentException($"{value} type ({value?.GetType()}) is not the same as the Field ({type.Name})", nameof(value));
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = _fields[binder.Name].Value;
        return true;
    }
}
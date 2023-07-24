// <copyright file="DynamicClassField.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Runtime;

public readonly struct DynamicClassField
{
    public DynamicClassField(string name, Type type, object value)
    {
        FieldName = name;
        FieldType = type;
        Value = value;
    }

    public string FieldName { get; }

    public Type FieldType { get; }

    public object Value { get; }
}
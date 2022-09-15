namespace ZirconNet.Core.Extensions;
public static class TypeExtension
{
    public static bool IsSameOrSubclassOf(this Type subType, Type baseType)
    {
        return subType.IsSubclassOf(baseType) || subType == baseType;
    }
}

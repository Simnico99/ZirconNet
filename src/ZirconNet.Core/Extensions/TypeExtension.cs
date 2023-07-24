// <copyright file="TypeExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Extensions;

public static class TypeExtension
{
    public static bool IsSameOrSubclassOf(this Type subType, Type baseType)
    {
        return subType.IsSubclassOf(baseType) || subType == baseType;
    }
}

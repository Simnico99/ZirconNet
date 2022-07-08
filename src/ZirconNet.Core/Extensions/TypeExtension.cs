using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Extensions;
public static class TypeExtension
{
    public static bool IsSameOrSubclassOf(this Type subType, Type baseType)
    {
        return subType.IsSubclassOf(baseType) || subType == baseType;
    }
}

// <copyright file="PageDataContextAttribute.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.WPF.Hosting;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PageDataContextAttribute : Attribute
{
    public PageDataContextAttribute()
    {
    }

    public PageDataContextAttribute(params Type[] pagesToBind)
    {
        PagesToBindType = pagesToBind;
    }

    public Type[] PagesToBindType { get; } = Array.Empty<Type>();
}

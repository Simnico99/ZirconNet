namespace ZirconNet.WPF.Hosting;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PageDataContextAttribute : Attribute
{
    public Type[] PagesToBindType { get; } = Array.Empty<Type>();
    public PageDataContextAttribute() { }

    public PageDataContextAttribute(params Type[] pagesToBind)
    {
        PagesToBindType = pagesToBind;
    }
}

namespace ZirconNet.WPF.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PageDataContextAttribute : Attribute
{
    public Type[]? PagesToBindType { get; }
    public PageDataContextAttribute() { }

    public PageDataContextAttribute(params Type[]? pagesToBind)
    {
        if (pagesToBind?.Length < 0 || pagesToBind is null)
        {
            return;
        }
        PagesToBindType = pagesToBind;
    }
}

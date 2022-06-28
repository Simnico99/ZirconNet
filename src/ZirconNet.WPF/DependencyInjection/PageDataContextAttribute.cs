namespace ZirconNet.WPF.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public class PageDataContextAttribute : Attribute
{
    public IEnumerable<string>? PagesToBindName { get; }

    public PageDataContextAttribute() { }

    public PageDataContextAttribute(params Type[]? pagesToBind)
    {

        if (pagesToBind?.Length < 0 || pagesToBind is null)
        {
            return;
        }
        PagesToBindName = pagesToBind.Select(x => x.Name).ToList();
    }
}

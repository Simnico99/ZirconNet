using ZirconNet.Core.Runtime;
using ZirconNet.WPF.DependencyInjection;
using ZirconNet.Core.Extensions;

namespace ZirconNet.WPF.Mvvm;
public class IocPage : Page
{
    public IocPage(IServiceProvider servicesProvider, IServiceCollection services)
    {
        var currentDataContexts = GetPageDataContexts(servicesProvider, services);
        var fields = new List<DynamicClassField>();

        foreach (var context in currentDataContexts)
        {
            var contextName = context.Key;
            var contextModel = context.Value;
            if (contextModel is not null && contextName is not null)
            {
                fields.Add(new DynamicClassField(contextName.Name, contextModel.GetType(), contextModel));
            }
        }

        var dynamicClass = new DynamicClass(fields);
        DataContext = dynamicClass;
    }

    private IEnumerable<KeyValuePair<Type?, ViewModel?>> GetPageDataContexts(IServiceProvider servicesProvider, IServiceCollection services)
    {
        foreach (var service in services)
        {
            if (!service.ServiceType.IsSameOrSubclassOf(typeof(ViewModel)))
            {
                continue;
            }

            var viewModel = (ViewModel?)servicesProvider.GetService(service.ServiceType);
            var attribute = viewModel?.GetType().GetCustomAttribute<PageDataContextAttribute>();

            if (attribute is not (not null and PageDataContextAttribute pageDataContextAttribute))
            {
                continue;
            }

            if (pageDataContextAttribute.PagesToBindName is null)
            {
                yield return new(viewModel?.GetType(), viewModel);
                continue;
            }

            if (pageDataContextAttribute.PagesToBindName.Contains(GetType()))
            {
                yield return new(viewModel?.GetType(), viewModel);
            }
        }
    }
}

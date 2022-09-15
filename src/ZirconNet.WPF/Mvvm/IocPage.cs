using ZirconNet.Core.Runtime;
using ZirconNet.WPF.DependencyInjection;
using ZirconNet.Core.Extensions;
using System.Collections.Generic;

namespace ZirconNet.WPF.Mvvm;
public class IocPage : Page
{
    private static ViewModel[] _servicesCache = Array.Empty<ViewModel>();

    public IocPage(IServiceProvider servicesProvider, IServiceCollection services)
    {
        var currentDataContexts = GetPageDataContexts(servicesProvider, services);
        var fields = new DynamicClassField[currentDataContexts.Length];

        for (var i = 0; i < currentDataContexts.Length; i++)
        {
            var context = currentDataContexts[i];
            var contextType = context.GetType();
            fields[i] = new DynamicClassField(contextType.Name, contextType, context);
        }

        var dynamicClass = new DynamicClass(fields);
        DataContext = dynamicClass;
    }

    private IEnumerable<ViewModel> GetPagesDataContextInternal(IServiceProvider servicesProvider, IServiceCollection services)
    {
        foreach (var service in services)
        {
            if (!service.ServiceType.IsSameOrSubclassOf(typeof(ViewModel)))
            {
                continue;
            }

            var viewModel = (ViewModel?)servicesProvider.GetService(service.ServiceType);
            var attribute = service.ServiceType.GetType().GetCustomAttribute<PageDataContextAttribute>();

            if (attribute is not (not null and PageDataContextAttribute pageDataContextAttribute))
            {
                continue;
            }

            if (viewModel is null)
            {
                continue;
            }

            if (pageDataContextAttribute.PagesToBindName is null)
            {
                yield return viewModel;
                continue;
            }
            if (pageDataContextAttribute.PagesToBindName.Contains(GetType().Name))
            {
                yield return viewModel;
            }
        }
    }

    public ViewModel[] GetPageDataContexts(IServiceProvider servicesProvider, IServiceCollection services)
    {
        if (_servicesCache.Length <= 0)
        {
            _servicesCache = GetPagesDataContextInternal(servicesProvider, services).ToArray();
            return _servicesCache;
        }
        return _servicesCache;
    }
}

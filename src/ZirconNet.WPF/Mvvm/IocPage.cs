using ZirconNet.Core.Runtime;
using ZirconNet.WPF.DependencyInjection;
using ZirconNet.Core.Extensions;
using System.Collections.Generic;

namespace ZirconNet.WPF.Mvvm;
public class IocPage : Page
{
    private static KeyValuePair<Type, ViewModel>[]? _servicesCache;

    public IocPage(IServiceProvider servicesProvider, IServiceCollection services)
    {
        var currentDataContexts = GetPageDataContexts(servicesProvider, services);
        var fields = new DynamicClassField[currentDataContexts.Length];

        for (var i = 0; i < currentDataContexts.Length; i++)
        {
            var context = currentDataContexts[i];
            fields[i] = new DynamicClassField(context.Key.Name, context.Key, context.Value);
        }

        var dynamicClass = new DynamicClass(fields);
        DataContext = dynamicClass;
    }

    private IEnumerable<KeyValuePair<Type, ViewModel>> GetPagesDataContextInternal(IServiceProvider servicesProvider, IServiceCollection services)
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

            if (viewModel is null)
            {
                continue;
            }

            if (pageDataContextAttribute.PagesToBindName is null || pageDataContextAttribute.PagesToBindName.Contains(GetType()))
            {
                yield return new(viewModel.GetType(), viewModel);
            }
        }
    }

    public KeyValuePair<Type, ViewModel>[] GetPageDataContexts(IServiceProvider servicesProvider, IServiceCollection services)
    {
        if (_servicesCache is null)
        {
            return _servicesCache = GetPagesDataContextInternal(servicesProvider, services).ToArray();
        }
        return _servicesCache;
    }
}

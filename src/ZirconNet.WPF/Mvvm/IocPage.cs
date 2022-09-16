using ZirconNet.Core.Runtime;
using ZirconNet.WPF.DependencyInjection;
using ZirconNet.Core.Extensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ZirconNet.WPF.Mvvm;
public class IocPage : Page
{
    public IocPage(IServiceProvider servicesProvider, IServiceCollection services)
    {
        var currentDataContexts = GetPageDataContexts(servicesProvider, services);
        var i = 0;
#if NET5_0_OR_GREATER
        var fields = new DynamicClassField[currentDataContexts.Count];
        foreach (var dataContext in CollectionsMarshal.AsSpan<ViewModel>(currentDataContexts))
        {
            var contextType = dataContext.GetType();
            fields[i++] = new DynamicClassField(contextType.Name, contextType, dataContext);
        }
#else
        var fields = new DynamicClassField[currentDataContexts.Count()];
        foreach (var dataContext in currentDataContexts)
        {
            var contextType = dataContext.GetType();
            fields[i++] = new DynamicClassField(contextType.Name, contextType, dataContext);
        }
#endif

        var dynamicClass = new DynamicClass(fields);
        DataContext = dynamicClass;
    }

#if NET5_0_OR_GREATER
    private List<ViewModel> GetPageDataContexts(IServiceProvider servicesProvider, IServiceCollection services)
    {
        var viewModels = new List<ViewModel>();

        foreach (var service in CollectionsMarshal.AsSpan(services.ToList()))
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

            if (pageDataContextAttribute.PagesToBindType is null)
            {
                viewModels.Add(viewModel);
                continue;
            }
            if (pageDataContextAttribute.PagesToBindType.Contains(GetType()))
            {
                viewModels.Add(viewModel);
            }
        }

        return viewModels;
#else
    private IEnumerable<ViewModel> GetPageDataContexts(IServiceProvider servicesProvider, IServiceCollection services)
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

            if (pageDataContextAttribute.PagesToBindType is null)
            {
                yield return viewModel;
                continue;
            }
            foreach (var type in pageDataContextAttribute.PagesToBindType)
            {
                if (type == GetType())
                {
                    yield return viewModel;
                    continue;
                }
            }
        }
#endif
    }
}

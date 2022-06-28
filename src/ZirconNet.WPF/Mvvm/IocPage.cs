﻿using ZirconNet.Core.Runtime;
using ZirconNet.WPF.DependencyInjection;

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
                fields.Add(new DynamicClassField(contextName, contextModel.GetType(), contextModel));
            }
        }

        var dynamicClass = new DynamicClass(fields);
        DataContext = dynamicClass;
    }
    private IEnumerable<KeyValuePair<string?, ViewModelBase?>> GetPageDataContexts(IServiceProvider servicesProvider, IServiceCollection services)
    {
        foreach (var service in services)
        {
            if (service.ServiceType.BaseType == typeof(ViewModelBase))
            {
                var viewModel = (ViewModelBase?)servicesProvider.GetService(service.ServiceType);
                var attribute = viewModel?.GetType().GetCustomAttribute<PageDataContextAttribute>();
                if (attribute is not null and PageDataContextAttribute pageDataContextAttribute)
                {
                    if (pageDataContextAttribute.PagesToBindName is null)
                    {
                        yield return new(viewModel?.GetType().Name, viewModel);
                    }

                    if (pageDataContextAttribute.PagesToBindName is not null && pageDataContextAttribute.PagesToBindName.Contains(GetType().Name))
                    {
                        yield return new(viewModel?.GetType().Name, viewModel);
                    }
                }
            }
        }
    }
}
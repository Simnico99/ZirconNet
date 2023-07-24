// <copyright file="IocPage.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Reflection;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ZirconNet.Core.Extensions;
using ZirconNet.Core.Runtime;
using ZirconNet.WPF.Hosting;

namespace ZirconNet.WPF.Mvvm;

public class IocPage : Page
{
    public IocPage(IServiceProvider servicesProvider, IServiceCollection services)
    {
        var dynamicClass = new DynamicClass(GetPageDataContexts(servicesProvider, services));
        DataContext = dynamicClass;
    }

    private IEnumerable<DynamicClassField> GetPageDataContexts(IServiceProvider servicesProvider, IServiceCollection services)
    {
        foreach (var service in services)
        {
            if (!service.ServiceType.IsSameOrSubclassOf(typeof(ViewModel)))
            {
                continue;
            }

            var viewModel = (ViewModel)servicesProvider.GetRequiredService(service.ServiceType);
            var viewModelType = viewModel.GetType();
            var attribute = viewModelType.GetCustomAttribute<PageDataContextAttribute>();

            if (attribute is not (not null and PageDataContextAttribute pageDataContextAttribute) || viewModel is null)
            {
                continue;
            }

            if (pageDataContextAttribute.PagesToBindType.Length <= 0 || pageDataContextAttribute.PagesToBindType.Contains(GetType()))
            {
                yield return new DynamicClassField(viewModelType.Name, viewModelType, viewModel);
                continue;
            }
        }
    }
}

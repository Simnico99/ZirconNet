using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace ZirconNet.WPF.Extensions;
public static class DependencyObjectExtension
{
    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null)
        {
            yield break;
        }

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            if (child is T t)
            {
                yield return t;
            }

            foreach (var childOfChild in FindVisualChildren<T>(child))
            {
                yield return childOfChild;
            }
        }
    }

    public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null)
        {
            yield break;
        }

        foreach (var rawChild in LogicalTreeHelper.GetChildren(depObj))
        {
            if (rawChild is null)
            {
                continue;
            }

            if (rawChild is not DependencyObject child)
            {
                continue;
            }

            if (child is T t)
            {
                yield return t;
            }

            foreach (T childOfChild in FindLogicalChildren<T>(child))
            {
                yield return childOfChild;
            }
        }
    }
}

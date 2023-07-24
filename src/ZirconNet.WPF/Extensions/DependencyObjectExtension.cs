// <copyright file="DependencyObjectExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Windows;
using System.Windows.Media;

namespace ZirconNet.WPF.Extensions;

public static class DependencyObjectExtension
{
    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj)
        where T : DependencyObject
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

    public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj)
        where T : DependencyObject
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

            foreach (var childOfChild in FindLogicalChildren<T>(child))
            {
                yield return childOfChild;
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Parallax.WPF;

internal static class TreeWalker
{
    public static IEnumerable<DependencyObject> GetVisualParents(this DependencyObject child)
    {
        yield return child;

        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null)
        {
            yield return parent;
            child = parent;
            parent = VisualTreeHelper.GetParent(child);
        }
    }

    //LogicalTreeHelper.GetParent
    public static IEnumerable<DependencyObject> GetLogicalParents(this DependencyObject child)
    {
        yield return child;

        var parent = LogicalTreeHelper.GetParent(child);
        while (parent != null)
        {
            yield return parent;
            child = parent;
            parent = LogicalTreeHelper.GetParent(child);
        }
    }

    public static Window? GetWindowFromElement(FrameworkElement element)
    {
        if (element == null)
            return null;

        return element.GetVisualParents()
            .OfType<Window>()
            .FirstOrDefault();
    }
}
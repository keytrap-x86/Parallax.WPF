using Microsoft.Xaml.Behaviors;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Parallax.WPF;

public class ParallaxEffect : Behavior<FrameworkElement>
{
    #region Dependency

    public static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterAttached("Parent", typeof(UIElement), typeof(ParallaxEffect), new PropertyMetadata(null));

    public static UIElement GetParent(DependencyObject obj) => (UIElement)obj.GetValue(ParentProperty);

    public static void SetParent(DependencyObject obj, UIElement value) => obj.SetValue(ParentProperty, value);

    public static readonly DependencyProperty XOffsetProperty
        = DependencyProperty.RegisterAttached("XOffset", typeof(int), typeof(ParallaxEffect), new PropertyMetadata(120));

    public static readonly DependencyProperty YOffsetProperty
        = DependencyProperty.RegisterAttached("YOffset", typeof(int), typeof(ParallaxEffect), new PropertyMetadata(120));

    public static int GetXOffset(DependencyObject obj) => (int)obj.GetValue(XOffsetProperty);

    public static void SetXOffset(DependencyObject obj, int value) => obj.SetValue(XOffsetProperty, value);

    public static int GetYOffset(DependencyObject obj) => (int)obj.GetValue(YOffsetProperty);

    public static void SetYOffset(DependencyObject obj, int value) => obj.SetValue(YOffsetProperty, value);

    #endregion Dependency

    private IDisposable _disposable;

    protected override void OnAttached()
    {
        AssociatedObject.Loaded += (a, b) => OnLoaded();

        var parent = GetParent(AssociatedObject);
        if (parent != null)
        {
            AttachToParent(parent);
        }

        UIElement bestParent = GetBestUiParent();
        if (bestParent == null)
        {
            return;
        }

        AttachToParent(bestParent);
    }

    private void AttachToParent(UIElement parent)
    {
        _disposable?.Dispose();

        parent.MouseMove += MouseMoveHandler;
        parent.MouseLeave += MouseLeaveHandler;
        parent.MouseEnter += MouseEnterHandler;

        _disposable = new ActionDisposable(() =>
        {
            parent.MouseMove -= MouseMoveHandler;
            parent.MouseLeave -= MouseLeaveHandler;
            parent.MouseEnter -= MouseEnterHandler;
        });
    }

    private UIElement GetBestUiParent()
    {
        var result = AssociatedObject.GetVisualParents()
            .OfType<UIElement>()
            .LastOrDefault();

        if (result != null)
        {
            return result;
        }

        result = AssociatedObject.GetLogicalParents()
            .OfType<UIElement>()
            .LastOrDefault();

        if (result != null)
        {
            return result;
        }

        return AssociatedObject;
    }

    private void OnLoaded()
    {
        var wnd = TreeWalker.GetWindowFromElement(AssociatedObject);
        if (wnd == null)
        {
            return;
        }

        AttachToParent(wnd);
    }

    protected override void OnDetaching()
        => _disposable?.Dispose();

    private async void MouseLeaveHandler(object sender, MouseEventArgs e)
    {
        // Transform back to original position
        if (AssociatedObject.RenderTransform is not TranslateTransform transform)
        {
            return;
        }

        var duration = TimeSpan.FromMilliseconds(500);
        var animation = new DoubleAnimation(0, duration);
        transform.BeginAnimation(TranslateTransform.XProperty, animation);
        transform.BeginAnimation(TranslateTransform.YProperty, animation);

        await Task.Delay(duration);

        transform.BeginAnimation(TranslateTransform.XProperty, null);
        transform.BeginAnimation(TranslateTransform.YProperty, null);

        transform.X = 0;
        transform.Y = 0;
    }

    private void MouseEnterHandler(object sender, MouseEventArgs e)
    {
        var mouse = e.GetPosition(AssociatedObject);
        var duration = TimeSpan.FromMilliseconds(200);

        TransformToMousePosition(mouse, duration);
    }

    private void MouseMoveHandler(object sender, MouseEventArgs e)
    {
        //HandleMousePosition(e.GetPosition(AssociatedObject));
        TransformToMousePosition(e.GetPosition(AssociatedObject));
    }

    private Point GetOffset(DependencyObject obj)
        => new Point(GetXOffset(obj), GetYOffset(obj));

    private Point GetNewPoint(Point mouse, Point offset)
    {
        double newX = AssociatedObject.ActualHeight - (mouse.X / offset.X) - AssociatedObject.ActualHeight;
        double newY = AssociatedObject.ActualWidth - (mouse.Y / offset.Y) - AssociatedObject.ActualWidth;
        return new Point(newX, newY);
    }

    private TranslateTransform GetTransform()
    {
        if (AssociatedObject.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            AssociatedObject.RenderTransform = transform;
        }

        return transform;
    }

    private async void TransformToMousePosition(Point mouse, TimeSpan? duration = null)
    {
        var offset = GetOffset(AssociatedObject);
        var newPoint = GetNewPoint(mouse, offset);

        var transform = GetTransform();

        if (duration.HasValue)
        {
            var animation = new DoubleAnimation(newPoint.X, duration.Value);
            transform.BeginAnimation(TranslateTransform.XProperty, animation);

            animation = new DoubleAnimation(newPoint.Y, duration.Value);
            transform.BeginAnimation(TranslateTransform.YProperty, animation);

            await Task.Delay(duration.Value);

            transform.BeginAnimation(TranslateTransform.XProperty, null);
            transform.BeginAnimation(TranslateTransform.YProperty, null);
        }
        else
        {
            if (transform.HasAnimatedProperties)
            {
                return;
            }
        }

        transform.X = newPoint.X;
        transform.Y = newPoint.Y;
    }
}
using Microsoft.Xaml.Behaviors;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Parallax.WPF;

public class ParallaxEffect : Behavior<FrameworkElement>
{
    #region Dependency

    public static readonly DependencyProperty UseParallaxProperty = DependencyProperty.RegisterAttached("UseParallax", typeof(bool), typeof(ParallaxEffect), new PropertyMetadata(false));
    public static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterAttached("Parent", typeof(UIElement), typeof(ParallaxEffect), new PropertyMetadata(null));
    public static readonly DependencyProperty IsBackgroundProperty = DependencyProperty.RegisterAttached("IsBackground", typeof(bool), typeof(ParallaxEffect), new PropertyMetadata(false));

    public static bool GetUseParallax(DependencyObject obj) => (bool)obj.GetValue(UseParallaxProperty);

    public static void SetUseParallax(DependencyObject obj, bool value) => obj.SetValue(UseParallaxProperty, value);

    public static bool GetIsBackground(DependencyObject obj) => (bool)obj.GetValue(IsBackgroundProperty);

    public static void SetIsBackground(DependencyObject obj, bool value) => obj.SetValue(IsBackgroundProperty, value);

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

        _disposable?.Dispose();

        if (!GetIsBackground(AssociatedObject))
        {
            AssociatedObject.MouseMove += MouseMoveHandler;
            _disposable = new ActionDisposable(() =>
            {
                AssociatedObject.MouseMove -= MouseMoveHandler;
            });
        }
        else
        {
            var parent = GetParent(AssociatedObject);
            parent.MouseMove += MouseMoveHandler;

            _disposable = new ActionDisposable(() =>
            {
                parent.MouseMove -= MouseMoveHandler;
            });
        }
    }

    private void OnLoaded()
    {
        var wnd = TreeWalker.GetWindowFromElement(AssociatedObject);
        if (wnd == null)
        {
            return;
        }

        _disposable?.Dispose();

        wnd.MouseMove += MouseMoveHandler;
        _disposable = new ActionDisposable(() =>
        {
            wnd.MouseMove -= MouseMoveHandler;
        });
    }

    protected override void OnDetaching() 
        => _disposable?.Dispose();

    private void MouseMoveHandler(object sender, MouseEventArgs e)
        => HandleMousePosition(e.GetPosition(AssociatedObject));

    private Point GetOffset(DependencyObject obj)
        => new Point(GetXOffset(obj), GetYOffset(obj));

    private void HandleMousePosition(Point mouse)
    {
        var offset = GetOffset(AssociatedObject);
        double newX = AssociatedObject.ActualHeight - (mouse.X / offset.X) - AssociatedObject.ActualHeight;
        double newY = AssociatedObject.ActualWidth - (mouse.Y / offset.Y) - AssociatedObject.ActualWidth;

        if (AssociatedObject.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform(newX, newY);
            AssociatedObject.RenderTransform = transform;
        }

        // Animate X if needed
        if (offset.X > 0)
        {
            transform.X = newX;
        }

        // Animate Y if needed
        if (offset.Y > 0)
        {
            transform.Y = newY;
        }
    }
}
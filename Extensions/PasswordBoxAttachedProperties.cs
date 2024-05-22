using System.Windows;
using System.Windows.Controls;

namespace Meet.Extensions;

public class PasswordBoxAttachedProperties
{
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordBoxAttachedProperties),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty AttachProperty =
        DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordBoxAttachedProperties),
            new PropertyMetadata(false, Attach));

    public static string GetPassword(DependencyObject obj)
    {
        return (string)obj.GetValue(PasswordProperty);
    }

    public static void SetPassword(DependencyObject obj, string value)
    {
        obj.SetValue(PasswordProperty, value);
    }

    public static bool GetAttach(DependencyObject obj)
    {
        return (bool)obj.GetValue(AttachProperty);
    }

    public static void SetAttach(DependencyObject obj, bool value)
    {
        obj.SetValue(AttachProperty, value);
    }

    private static void Attach(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (!(obj is PasswordBox passwordBox))
            return;

        if ((bool)e.NewValue)
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        else
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (!(sender is PasswordBox passwordBox))
            return;

        SetPassword(passwordBox, passwordBox.Password);
    }
}
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Meet.Extensions;

internal class PasswordBoxExtension
{
    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MyPropertyProperty =
        DependencyProperty.RegisterAttached("GetPwd", typeof(int), typeof(PasswordBoxExtension),
            new PropertyMetadata("", OnPwdChanged));

    //password属性扩充
    public static string GetPwd(DependencyObject obj)
    {
        return (string)obj.GetValue(MyPropertyProperty);
    }

    public static void SetPwd(DependencyObject obj, string value)
    {
        obj.SetValue(MyPropertyProperty, value);
    }

    private static void OnPwdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = d as PasswordBox;
        var pwd = (string)e.NewValue; //获取现在输入的值（新值
        if (passwordBox != null && !passwordBox.Equals(pwd)) passwordBox.Password = pwd;
    }

    //Password行为，Password变化自定义附加属性跟着变化
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        //附加注入事件
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += OnPwdChanged;
        }

        //password变化，自定义附加属性跟着变化
        private void OnPwdChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var password = GetPwd(passwordBox);
            if (passwordBox != null && !passwordBox.Equals(passwordBox)) SetPwd(passwordBox, passwordBox.Password);
        }

        //销毁移除事件
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= OnPwdChanged;
        }
    }
}
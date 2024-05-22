using System.Windows;

namespace Meet.Extensions;

public static class ButtonProperties
{
    // 定义附加属性DeleteParameter
    public static readonly DependencyProperty DeleteParameterProperty =
        DependencyProperty.RegisterAttached(
            "DeleteParameter",
            typeof(object),
            typeof(ButtonProperties),
            new PropertyMetadata(null));

    // 设置附加属性的方法
    public static void SetDeleteParameter(UIElement element, object value)
    {
        element.SetValue(DeleteParameterProperty, value);
    }

    // 获取附加属性的方法
    public static object GetDeleteParameter(UIElement element)
    {
        return element.GetValue(DeleteParameterProperty);
    }
}
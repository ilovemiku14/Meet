namespace Meet.Models;

internal class MenuInfo
{
    public MenuInfo(string icon, string menuName, string viewName)
    {
        Icon = icon;
        MenuName = menuName;
        ViewName = viewName;
    }

    public string Icon { get; set; }
    public string MenuName { get; set; }
    public string ViewName { get; set; }
}
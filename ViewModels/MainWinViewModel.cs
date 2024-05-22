using System.Diagnostics;
using Meet.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace Meet.ViewModels;

internal class MainWinViewModel : BindableBase, IDialogAware
{
    //区域对象
    private readonly IRegionManager RegionManager;

    private List<MenuInfo> _MenuInfoList;
    private readonly List<MenuInfo> historyMenuInfoList = new();

    //记录当前跳转的页面的位置
    private int indexPageNum = 1;

    public MainWinViewModel(IRegionManager regionManager)
    {
        MenuInfoList = new List<MenuInfo>();
        MenuInfoList.Add(new MenuInfo("Home", "主页", "HomeUC"));
        MenuInfoList.Add(new MenuInfo("Home", "我的预订", "MyReservationUC"));
        // MenuInfoList.Add(new MenuInfo("Home", "查看预定", "ViewReservationsUC"));
        MenuInfoList.Add(new MenuInfo("Home", "预定会议", "ScheduleMeetingUC"));
        // MenuInfoList.Add(new MenuInfo("Home", "会议室", "MeetRoomUC"));
        // ItemName = new ObservableCollection<string>{ "主页", "我的预订", "查看预定", "预定会议", "会议室" };
        historyMenuInfoList.Add(new MenuInfo("Home", "主页", "HomeUC"));
        //默认关闭侧边栏
        isLeftMenu = false;
        //区域对象实例化
        RegionManager = regionManager;
        NavigateCmm = new DelegateCommand<MenuInfo>(Navigate);
        MovePrevCommand = new DelegateCommand(_MovePrevCommand);
        MoveNextCommand = new DelegateCommand(_MoveNextCommand);
    }

    //下列列表
    public List<MenuInfo> MenuInfoList
    {
        get => _MenuInfoList;
        set
        {
            _MenuInfoList = value;
            RaisePropertyChanged();
        }
    }

    public DelegateCommand<MenuInfo> NavigateCmm { get; set; }

    //控制侧边栏的开关
    private bool _isLeftMenu { get; set; }

    public bool isLeftMenu
    {
        get => _isLeftMenu;
        set
        {
            _isLeftMenu = value;
            RaisePropertyChanged();
        }
    }

    //路由中的左右按钮
    public DelegateCommand MovePrevCommand { get; set; }

    //路由中的左右按钮
    public DelegateCommand MoveNextCommand { get; set; }
    public string Title => "主页";

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog()
    {
        throw new NotImplementedException();
    }

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
    }

    private void Navigate(MenuInfo menuInfo)
    {
        if (isLeftMenu)
            isLeftMenu = false;
        else
            isLeftMenu = true;
        if (menuInfo == null || string.IsNullOrEmpty(menuInfo.MenuName)) return;
        RegionManager.Regions["MainViewRegion"].RequestNavigate(menuInfo.ViewName);
        indexPageNum++;
        historyMenuInfoList.Add(menuInfo);
    }

    private void _MovePrevCommand()
    {
        if (historyMenuInfoList.Count == 0 || indexPageNum <= 0) return;
        indexPageNum--;
        RegionManager.Regions["MainViewRegion"].RequestNavigate(historyMenuInfoList.ElementAt(indexPageNum).ViewName);
        Debug.WriteLine("1111" + historyMenuInfoList.Count);
    }

    private void _MoveNextCommand()
    {
        if (historyMenuInfoList.Count == 0 || indexPageNum <= historyMenuInfoList.Count) return;
        indexPageNum++;
        RegionManager.Regions["MainViewRegion"].RequestNavigate(historyMenuInfoList.ElementAt(indexPageNum).ViewName);
        Debug.WriteLine("2222" + historyMenuInfoList.Count);
    }
    ////菜单栏的切换函数
    //private void SelectedPersonChanged()
    //{
    //    Debug.WriteLine(_selectedPerson);
    //    // 在这里处理选中项的值
    //    if (_selectedPerson != null)
    //    {

    //    }
    //}
    ////page来
    //public void OnNavigatedFrom(NavigationParameters parameters)
    //{
    //}
    ////page去
    //public void OnNavigatedTo(NavigationParameters parameters)
    //{
    //}
}
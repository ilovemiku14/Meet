using System.Windows;
using DryIoc;
using Meet.HttpClients;
using Meet.ViewModels;
using Meet.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;

namespace Meet;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    private IRegionManager _regionManager;

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWin>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _regionManager.Regions["MainViewRegion"].RequestNavigate("HomeUC");
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        //默认登陆页面
        containerRegistry.RegisterDialog<LoginUC, LoginUCViewModel>();
        // containerRegistry.RegisterDialog<MainWin, MainWinViewModel>();
        //注入api接口
        containerRegistry.GetContainer()
            .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webURI"));
        containerRegistry.RegisterForNavigation<HomeUC, HomeUCViewModel>();
        containerRegistry.RegisterForNavigation<MeetRoomUC, MeetRoomUCViewModel>();
        containerRegistry.RegisterForNavigation<MyReservationUC, MyReservationUCViewModel>();
        containerRegistry.RegisterForNavigation<ScheduleMeetingUC, ScheduleMeetingUCViewModel>();
        containerRegistry.RegisterForNavigation<ViewReservationsUC, ViewReservationsUCViewModel>();
        containerRegistry.RegisterSingleton<MainWinViewModel>();

        //containerRegistry.RegisterForNavigation<Homepage>("Homepage");
        //containerRegistry.RegisterForNavigation<MeetRoom>("MeetRoom");
        //containerRegistry.RegisterForNavigation<MyReservation>("MyReservation");
        //containerRegistry.RegisterForNavigation<ScheduleMeeting>("ScheduleMeeting");
        //containerRegistry.RegisterForNavigation<ViewReservations>("ViewReservations");
    }

    //初始化
    protected override void OnInitialized()
    {
        //var dlalog = Container.Resolve<IDialogService>();
        //dlalog.ShowDialog("LoginUC", callback =>
        //{
        //    if (callback.Result != ButtonResult.OK)
        //    {
        //        Environment.Exit(0);
        //    }
        //});
        base.OnInitialized();
        _regionManager = Container.Resolve<IRegionManager>();
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ImTools;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using Meet.HttpClients;
using Meet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using RestSharp;


namespace Meet.ViewModels;

internal class MyReservationUCViewModel : BindableBase, IDialogAware,INotifyPropertyChanged,ICommand
{
    public string Title => throw new NotImplementedException();

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog()
    {
        throw new NotImplementedException();
    }

    public void OnDialogClosed()
    {
        throw new NotImplementedException();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        throw new NotImplementedException();
    }
    public readonly HttpRestClient httpClients;
    public MyReservationUCViewModel(HttpRestClient _httpClients)
    {
        httpClients = _httpClients;
        //初始化数据
        MeetList = CreateData();
        //更新对象初始化
        UpdateMeetCmm = new DelegateCommand(UpdateMeetList);
        //查询对象初始化
        QueriedMeetCollectionCmm =new DelegateCommand(QueriedMeetCollection);
        //清除查询条件初始化
        ClearQueriedMeetCollectionCmm =new DelegateCommand(ClearQueriedMeetCollection);
        //删除按钮初始化
         DeleteMeetCmm = new RelayCommand(Execute);
    }

    public RelayCommand DeleteMeetCmm{ get; set; }
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        // 判断是否能执行命令，可能需要检查参数
        return parameter != null;
    }

    public void Execute(object parameter)
    {
        // 参数parameter即为从附加属性传入的Meet值
        Models.Meet meet = parameter as Models.Meet;
        var deleteMeet = new ApiRequest();
        deleteMeet.method = Method.POST;
        deleteMeet.route = "api/MeetControllers/DeleteMeet";
        deleteMeet.Parameters = meet;
        var s = httpClients.Excute(deleteMeet);
        Application.Current.Dispatcher.Invoke(() =>
        {
            MeetList = CreateData();
            OnPropertyChanged(nameof(MeetList));
        });
    }
    private string _cueryConditions;
    public string  CueryConditions
    {
        get { return _cueryConditions; }
        set
        {
            if (_cueryConditions != value)
            {
                _cueryConditions = value;
                OnPropertyChanged(nameof(_cueryConditions));
            }
        }
    }
    //去除查询的结果函数
    public DelegateCommand ClearQueriedMeetCollectionCmm { get; set; }

    public void ClearQueriedMeetCollection()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            CueryConditions = "";
            MeetList = CreateData();
            OnPropertyChanged(nameof(MeetList));
            OnPropertyChanged(nameof(CueryConditions));
        });
    }
    //查询函数
    public DelegateCommand QueriedMeetCollectionCmm { get; set; }

    public void QueriedMeetCollection()
    {
        if (CueryConditions != null)
        {
            List<Models.Meet> queryResults = MeetList.Where(meet => 
                meet.MeetTitle.Contains(CueryConditions) || 
                meet.MeetMessage.Contains(CueryConditions) || 
                meet.Address.Contains(CueryConditions) || 
                meet.MeetingRoom.Contains(CueryConditions)  
            ).ToList();
            ObservableCollection<Models.Meet> resMeet = new ObservableCollection<Models.Meet>();
            foreach (Models.Meet meet in queryResults)
            {
                resMeet.Add(meet);
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                MeetList = resMeet;
                OnPropertyChanged(nameof(MeetList));
            });
        }
    }
    //更新函数
    public DelegateCommand UpdateMeetCmm { get; set; }

    public void UpdateMeetList()
    {
        var addMeets = new ApiRequest();
        addMeets.method = Method.PUT;
        addMeets.route = "api/MeetControllers/UpdateMeets";
        addMeets.Parameters = MeetList;
        httpClients.Excute(addMeets);
        ObservableCollection<Models.Meet> resMeet = CreateData();
        Application.Current.Dispatcher.Invoke(() =>
        {
            MeetList = resMeet;
            OnPropertyChanged(nameof(MeetList));
        });
    }
   //获取当前房间号
    private string _meetingRoom ;
    public string MeetingRoom
    {
        get { return _meetingRoom; }
        set
        {
            if (_meetingRoom != value)
            {
                _meetingRoom = value;
                OnPropertyChanged(nameof(_meetingRoom));
            }
        }
    }
    private ObservableCollection<Models.Meet> _meetList = new ObservableCollection<Models.Meet>();
    public ObservableCollection<Models.Meet> MeetList
    {
        get => _meetList;
        set
        {
            if (_meetList != value)
            {
                _meetList = value;
                OnPropertyChanged(nameof(_meetList));
            }
        }
    }
    
    private  ObservableCollection<Models.Meet> CreateData()
    {
        var addMeets = new ApiRequest();
        addMeets.method = Method.GET;
        addMeets.route = "api/MeetControllers/GetgMeetAppointmentList?Address=武汉&Aid="+LoginUCViewModel.AccountInfoMsg.AccountId;
        var MeetListsResponse = httpClients.Excute(addMeets);
        JArray  jArray = (JArray)MeetListsResponse.ResultData;
        ObservableCollection<Models.Meet> meetList  = jArray.ToObject<ObservableCollection<Models.Meet>>();
        for (int i = 0; i < meetList.Count; i++)
        {
            meetList[i].MeetSortId = i;
        }
        return meetList;
    }
    public event PropertyChangedEventHandler PropertyChanged;
    
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;
 
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }
 
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
 
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
 
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
    //对象绑定
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
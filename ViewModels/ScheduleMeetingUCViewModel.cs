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

namespace Meet.ViewModels
{
    internal class ScheduleMeetingUCViewModel : BindableBase, IDialogAware,INotifyPropertyChanged
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
        public event PropertyChangedEventHandler PropertyChanged;
        //日期对象
        private ObservableCollection<DayModel> _days;
        public ObservableCollection<DayModel> Days
        {
            get { return _days; }
            set { _days = value; OnPropertyChanged(nameof(Days)); }
        }

        public ICommand DayClickCommand { get; private set; }
        public readonly HttpRestClient httpClients;
        // Call this method to initialize the days for a given month
        public ScheduleMeetingUCViewModel(HttpRestClient _httpClients)
        {
            _controlsEnabled = true;
            SelectedIndex = 0;
            // 初始化数据源
            AddressList = new ObservableCollection<string>
            {
                "月",
                "日"
            };
            //初始化时间
            _meetStartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _regularMeetStartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _regularMeetEndTimeSpan = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _meetEndDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            // 初始化命令
            DayClickCommand = new RelayCommand(DayClicked);
            
            // 假设传入了月份，这里初始化天数
            int month = DateTime.Now.Month;
            InitializeDays(month);
            ////添加会议
            MeetAddCommand = new DelegateCommand(MeetAdd);
            httpClients = _httpClients;
            //默认日期默认房间号
            MeetingRomm = "502B TV";
            SelectedMonth = (DateTime.Now).ToString("yyyy-MM");
            GetMeetingsForMonth(SelectedMonth,MeetingRomm);

            //创建一个定时器，每秒更新一次 RealTimePageData
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += GetRealTimePageData;
            timer.Start();
            //选择会议房间号
            TreeViewItemClickedCommand = new DelegateCommand<string>(OnTreeViewItemClicked);
            //设置当前月份前后各一年的月份
            SetComboboxMonth();
            //设置每个会议button的点击事件
            ShowMeetInfoCommand = new DelegateCommand<Models.Meet>(ShowMeetInfo);
        }
        //设置每个会议button的点击事件
        public DelegateCommand<Models.Meet> ShowMeetInfoCommand { get; private set; }
        private void ShowMeetInfo(Models.Meet meet)
        {
            Console.WriteLine(111);
            MessageBox.Show("登陆成功");
            // 在这里弹出对话框，显示会议的信息
        }
        //绑定月份选择下拉框的对象
        private ObservableCollection<string> _months;
        public ObservableCollection<string> Months
        {
            get => _months;
            set
            {
                if (_months != value)
                {
                    _months = value;
                    OnPropertyChanged(nameof(_months));
                }
            }
        }
        private string _selectedMonth;
        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = value;
                    OnPropertyChanged(nameof(_selectedMonth));
                    GetMeetingsForMonth(SelectedMonth,MeetingRomm);
                }
            }
        }
        public void SetComboboxMonth()
        {
            Months = new ObservableCollection<string>();
            var currentYear = DateTime.Now.Year;
            for (int year = currentYear - 1; year <= currentYear + 1; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    Months.Add(new DateTime(year, month, 1).ToString("yyyy-MM"));
                }
            }
        }
        //当前选择的会议室房间号
        public string MeetingRomm;
        public DelegateCommand<string> TreeViewItemClickedCommand { get; private set; }
        private void OnTreeViewItemClicked(string itemText)
        {
            // 处理点击TreeViewItem事件，itemText为所选TreeViewItem的文本值
            //MessageBox.Show($"Clicked TreeViewItem: {itemText}");
            MeetingRomm = itemText;
            GetMeetingsForMonth(SelectedMonth, MeetingRomm);
        }
        //获取当前时间的定时函数
        private  void GetRealTimePageData(object sender, EventArgs e)
        {
            
            string currentTime = "当前时间:" + DateTime.Now.Year + "年" +
                              DateTime.Now.Month + "月" + DateTime.Now.Day + "日" + DateTime.Now.Hour + "点" +
                              DateTime.Now.Minute + "分" + DateTime.Now.Second + "秒"; 
            Application.Current.Dispatcher.Invoke(() =>
            {
                RealTimePageData = currentTime;
                OnPropertyChanged(nameof(RealTimePageData));
            });
        }
        //获取当前时间的定时函数的对象
        private string _realTimePageData ;
        public string RealTimePageData
        {
            get { return _realTimePageData; }
            set
            {
                if (_realTimePageData != value)
                {
                    _realTimePageData = value;
                    OnPropertyChanged(nameof(_realTimePageData));
                }
            }
        }
        public void GetMeetingsForMonth(string dateTime,string meetingRoom)
        {
            string format = "yyyy-MM";
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime result = DateTime.ParseExact(dateTime, format, provider);
            InitializeDays(result.Month);
            DateTime firstDayOfMonth = new DateTime(result.Year, result.Month, 1);
            var addMeets = new ApiRequest();
            addMeets.method = Method.GET;
            addMeets.route = "api/MeetControllers/GetMeetingsForMonth?currentMonth="+firstDayOfMonth+"&Address=武汉&MeetingRoom="+meetingRoom;
            var addMeetsResponse = httpClients.Excute(addMeets);
            JArray  jArray = (JArray)addMeetsResponse.ResultData;
            List<Models.Meet> meetList  = jArray.ToObject<List<Models.Meet>>();
            Dictionary<int, List<Models.Meet>> newDictionary = new Dictionary<int, List<Models.Meet>>();
            foreach (DayModel dayModel in Days)
            {
                newDictionary[dayModel.Day] = new List<Models.Meet>();
            }
            foreach (var meeting in meetList)
            {
                int day = meeting.MeetStartTime.Day;
                if (!newDictionary.ContainsKey(day))
                    newDictionary[day] = new List<Models.Meet>();
                var selectAccount = new ApiRequest();
                selectAccount.method = Method.GET;
                selectAccount.route = "api/MeetingUserRelationship/GetMeetingCreatorByMeetingId?meetingId="+meeting.MeetId;
                var selectAccountResponse = httpClients.Excute(selectAccount);
                JObject  jObject = (JObject)selectAccountResponse.ResultData;
                AccountInfo AccountInfo  = jObject.ToObject<AccountInfo>();
                meeting.MeetText = "会议名称:"+meeting.MeetTitle+"\r\n会议开始时间:"+meeting.MeetStartTime+"" +
                                   "\r\n会议结束时间:"+meeting.MeetEndTime+"\r\n会议描述:"+meeting.MeetMessage+"\r\n会议室:"
                                   +meeting.MeetingRoom+"\r\n创建人:"+AccountInfo.Name+"\r\n创建人邮箱:"+AccountInfo.DxcGmail;
                newDictionary[day].Add(meeting);
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                MeetingsByDay = newDictionary;
                OnPropertyChanged(nameof(MeetingsByDay));
            });
        }
        private Dictionary<int, List<Models.Meet>> _meetingsByDay = new Dictionary<int, List<Models.Meet>>();
        public Dictionary<int, List<Models.Meet>> MeetingsByDay
        {
            get { return _meetingsByDay; }
            set
            {
                if (_meetingsByDay != value)
                {
                    _meetingsByDay = value;
                    OnPropertyChanged(nameof(_meetingsByDay));
                }
            }
        }
        //会议结束时间
        private DateTime _regularMeetStartDateTime;
        private DateTime  _regularMeetEndTimeSpan;
        public DateTime RegularMeetStartDateTime
        {
            get { return _regularMeetStartDateTime; }
            set
            {
                if (_regularMeetStartDateTime != value)
                {
                    _regularMeetStartDateTime = value;
                    OnPropertyChanged(nameof(_regularMeetStartDateTime));
                    Console.WriteLine(_regularMeetStartDateTime);
                }
            }
        }
        public DateTime  RegularMeetEndTimeSpan
        {
            get { return _regularMeetEndTimeSpan; }
            set
            {
                if (_regularMeetEndTimeSpan != value)
                {
                    _regularMeetEndTimeSpan = value;
                    OnPropertyChanged(nameof(_regularMeetEndTimeSpan));
                    Console.WriteLine(_regularMeetEndTimeSpan);
                }
            }
        }
        //定期会议放在是星期几
        private string _regularMeetDate;
        public string RegularMeetDate
        {
            get { return _regularMeetDate; }
            set
            {
                if (_regularMeetDate != value)
                {
                    _regularMeetDate = value;
                    OnPropertyChanged(nameof(_regularMeetDate));
                    Console.WriteLine(_regularMeetDate);
                }
            }
        }
        //是否开启定期会议
        private bool _isRegularMeet;
        public bool IsRegularMeet
        {
            get { return _isRegularMeet; }
            set
            {
                if (_isRegularMeet != value)
                {
                    _isRegularMeet = value;
                    OnPropertyChanged(nameof(_isRegularMeet));
                }
            }
        }
        //会议详述
        private string _meetMessage;
        public string MeetMessage
        {
            get { return _meetMessage; }
            set
            {
                if (_meetMessage != value)
                {
                    _meetMessage = value;
                    OnPropertyChanged(nameof(_meetMessage));
                }
            }
        }
        //会议地址
        private string _meetingRoom;
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
        //会议标题
        private string _meetTitle;
        public string MeetTitle
        {
            get { return _meetTitle; }
            set
            {
                if (_meetTitle != value)
                {
                    _meetTitle = value;
                    OnPropertyChanged(nameof(_meetTitle));
                }
            }
        }
        //会议结束时间
        private DateTime _meetEndDateTime;
        private DateTime  _meetEndTimeSpan;
        public DateTime MeetEndDataTime
        {
            get { return _meetEndDateTime; }
            set
            {
                if (_meetEndDateTime != value)
                {
                    _meetEndDateTime = value;
                    OnPropertyChanged(nameof(_meetEndDateTime));
                }
            }
        }
        public DateTime  MeetEndTimeSpan
        {
            get { return _meetEndTimeSpan; }
            set
            {
                if (_meetEndTimeSpan != value)
                {
                    _meetEndTimeSpan = value;
                    OnPropertyChanged(nameof(_meetEndTimeSpan));
                }
            }
        }
        //会议开始时间
        private DateTime _meetStartDateTime;
        private DateTime  _meetStartTimeSpan;
        public DateTime MeetStartDataTime
        {
            get { return _meetStartDateTime; }
            set
            {
                if (_meetStartDateTime != value)
                {
                    _meetStartDateTime = value;
                    OnPropertyChanged(nameof(_meetStartDateTime));
                }
            }
        }
        public DateTime  MeetStartTimeSpan
        {
            get { return _meetStartTimeSpan; }
            set
            {
                if (_meetStartTimeSpan != value)
                {
                    _meetStartTimeSpan = value;
                    OnPropertyChanged(nameof(_meetStartTimeSpan));
                }
            }
        }
        //添加会议函数
        public DelegateCommand MeetAddCommand { get; private set; }
        private void MeetAdd()
        {
            List<DateTime> regularMeets = new List<DateTime>();
            DialogHost.CloseDialogCommand.Execute(null, null);
            if (_isRegularMeet)
            {
                regularMeets = GetSpecifiedDaysOfWeekBetweenDates(RegularMeetStartDateTime,RegularMeetEndTimeSpan,RegularMeetDate);
                List<Models.Meet> meets = new List<Models.Meet>();
                foreach (DateTime regularMeet in regularMeets)
                {
                    string[] parts = MeetingRoom.Split(':');
                    string meetingRoomStr = parts[parts.Length - 1].Trim(); // 使用Trim移除前后的空格
                    Models.Meet meet = new Models.Meet();
                    meet.Address = "武汉";
                    meet.MeetTitle = MeetTitle;
                    meet.MeetMessage = MeetMessage;
                    meet.MeetingRoom = meetingRoomStr;
                    meet.MeetStartTime = new DateTime(regularMeet.Year, regularMeet.Month
                        , regularMeet.Day, MeetStartTimeSpan.Hour, MeetStartTimeSpan.Minute, 0);
                    meet.MeetEndTime = new DateTime(regularMeet.Year, regularMeet.Month
                        , regularMeet.Day, MeetEndTimeSpan.Hour, MeetEndTimeSpan.Minute, 0);
                    meet.MeetCreateTime = DateTime.Now;
                    meet.Regular = true;
                    meets.Add(meet);
                }
                
                var addMeets = new ApiRequest();
                addMeets.method = Method.POST;
                addMeets.route = "api/MeetControllers/AddMeets";
                addMeets.Parameters = meets;
                ApiResponse addMeetsResponse = httpClients.Excute(addMeets);
                JArray  jArray = (JArray)addMeetsResponse.ResultData;
                List<Models.Meet> meetList  = jArray.ToObject<List<Models.Meet>>();
                List<MeetingUserRelationship> meetingUserRelationshipList = new List<MeetingUserRelationship>();
                foreach (Models.Meet meet in meetList)
                {
                    MeetingUserRelationship meetingUserRelationships = new MeetingUserRelationship();
                    meetingUserRelationships.Aid = LoginUCViewModel.AccountInfoMsg.AccountId;
                    meetingUserRelationships.Mid = Convert.ToInt32(meet.MeetId);
                    meetingUserRelationshipList.Add(meetingUserRelationships);
                }
                var meetingUserRelationship = new ApiRequest();
                meetingUserRelationship.method = Method.POST;
                meetingUserRelationship.route = "api/MeetingUserRelationship/AddMeetingUserRelationships";
                meetingUserRelationship.Parameters = meetingUserRelationshipList;
                var meetingUserRelationshipResponse = httpClients.Excute(meetingUserRelationship);
            }
            else
            {
                string[] parts = MeetingRoom.Split(':');
                string meetingRoomStr = parts[parts.Length - 1].Trim();
                Models.Meet meet = new Models.Meet();
                meet.Address = "武汉";
                meet.MeetTitle = MeetTitle;
                meet.MeetMessage = MeetMessage;
                meet.MeetingRoom = meetingRoomStr;
                meet.MeetStartTime = new DateTime(MeetStartDataTime.Year, MeetStartDataTime.Month
                    , MeetStartDataTime.Day, MeetStartTimeSpan.Hour, MeetStartTimeSpan.Minute, 0);
                meet.MeetEndTime = new DateTime(MeetEndDataTime.Year, MeetEndDataTime.Month
                    , MeetEndDataTime.Day, MeetEndTimeSpan.Hour, MeetEndTimeSpan.Minute, 0);
                meet.MeetCreateTime = DateTime.Now;
                meet.Regular = false;
                var api = new ApiRequest();
                api.method = Method.POST;
                api.route = "api/MeetControllers/AddMeet";
                api.Parameters = meet;
                var response = httpClients.Excute(api);
                if (response.ResultCode != 200)
                {
                    return;
                }
                JObject  jObject = (JObject)response.ResultData;
                Models.Meet meetreq  = jObject.ToObject<Models.Meet>();
                MeetingUserRelationship meetingUserRelationship = new MeetingUserRelationship();
                meetingUserRelationship.Aid = LoginUCViewModel.AccountInfoMsg.AccountId;
                meetingUserRelationship.Mid = meetreq.MeetId;
                var meetingUserRelationshipRequest = new ApiRequest();
                meetingUserRelationshipRequest.method = Method.POST;
                meetingUserRelationshipRequest.route = "api/MeetingUserRelationship/AddMeetingUserRelationship";
                meetingUserRelationshipRequest.Parameters = meetingUserRelationship;
                var meetingUserRelationshipResponse = httpClients.Excute(meetingUserRelationshipRequest);
            }
        }
        static DayOfWeek GetDayOfWeekFromChinese(string dayOfWeek)
        {
            string[] parts = dayOfWeek.Split(':');
            string result = parts[parts.Length - 1].Trim(); // 使用Trim移除前后的空格
            switch (result)
            {
                case "星期一":
                    return DayOfWeek.Monday;
                case "星期二":
                    return DayOfWeek.Tuesday;
                case "星期三":
                    return DayOfWeek.Wednesday;
                case "星期四":
                    return DayOfWeek.Thursday;
                case "星期五":
                    return DayOfWeek.Friday;
                case "星期六":
                    return DayOfWeek.Saturday;
                case "星期日":
                    return DayOfWeek.Sunday;
                default:
                    throw new ArgumentException("无效的星期几字符串");
            }
        }
        static List<DateTime> GetSpecifiedDaysOfWeekBetweenDates(DateTime startDate, DateTime endDate, string dayOfWeek)
        {
            List<DateTime> specifiedDays = new List<DateTime>();

            // 将中文星期转换为DayOfWeek枚举值
            DayOfWeek specifiedDayOfWeek = GetDayOfWeekFromChinese(dayOfWeek);

            DateTime current = startDate;
            while (current <= endDate)
            {
                if (current.DayOfWeek == specifiedDayOfWeek)
                {
                    specifiedDays.Add(current);
                }
                current = current.AddDays(1);
            }

            return specifiedDays;
        }
        //是否开启添加按钮
        private bool _controlsEnabled;
        public bool ControlsEnabled
        {
            get { return _controlsEnabled; }
            set
            {
                if (_controlsEnabled != value)
                {
                    _controlsEnabled = value;
                    OnPropertyChanged(nameof(_controlsEnabled));
                }
            }
        }
        //年月日下列列表
        private ObservableCollection<string> _addressList;
        public ObservableCollection<string> AddressList
        {
            get => _addressList;
            set
            {
                if (_addressList != value)
                {
                    _addressList = value;
                    OnPropertyChanged(nameof(SelectedIndex));
                }
            }
        }
        private string _selectedDate;
        //下列列表获取年月日切换
        public string SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedIndex));
                    if (value.Equals("日"))
                    {
                        SelectedIndex = 1;
                    }
                    else
                    {
                        SelectedIndex = 0;
                    }
                }
                
            }
        }
        //获取页面显示日期
        private void InitializeDays(int month)
        {
            Days = null;
            Days = new ObservableCollection<DayModel>();

            // 获取当前月份的第一天
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, month, 1);

            // 获取当前月份的第一天是星期几
           // DayOfWeek firstDayOfWeek = firstDayOfMonth.DayOfWeek;

            // 计算需要添加到集合中的上个月日期号的数量
            //int daysToAddFromPreviousMonth = ((int)firstDayOfWeek + 6) % 7;

            // 获取上个月的最后一天
            //DateTime lastDayOfPreviousMonth = firstDayOfMonth.AddDays(-1);

            // 添加上个月的日期号到集合中
            // for (int i = daysToAddFromPreviousMonth; i > 0; i--)
            // {
            //     Days.Insert(0, new DayModel { Day = lastDayOfPreviousMonth.Day });
            //     lastDayOfPreviousMonth = lastDayOfPreviousMonth.AddDays(-1);
            // }

            // 添加当前月份的日期号到集合中
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                Days.Add(new DayModel { Day = i });
            }
        }
        //点击日期函数
        private void DayClicked(object parameter)
        {
            if (parameter is DayModel day)
            {
                SelectedIndex  = 1;
                // 实现点击逻辑
            }
        }

        //切换日和月日历的对象
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged(nameof(SelectedIndex));
                }
            }
        }
        
        //日期对象
        public class DayModel
        {
            public int Day { get; set; }
        }
        //RelayCommand实现
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
}

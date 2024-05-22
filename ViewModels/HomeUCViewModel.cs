using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
    internal class HomeUCViewModel : BindableBase, IDialogAware,INotifyPropertyChanged
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
        public HomeUCViewModel(HttpRestClient _httpClients)
        {
            httpClients = _httpClients;
            //设置当前月份前后各一年的月份
            SetComboboxMonth();
            SelectedMonth = (DateTime.Now).ToString("yyyy-MM");
            CreateData(SelectedMonth);
        }
        //获取月份的每一天
        public string[] GenerateDaysInMonth(string month)
        {
            // 解析传入的月份字符串
            if (!DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedMonth))
            {
                throw new ArgumentException("Invalid month format. Please use yyyy-MM format.");
            }

            // 获取该月份的第一天和下个月的第一天
            DateTime firstDayOfMonth = new DateTime(parsedMonth.Year, parsedMonth.Month, 1);
            DateTime firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            // 生成日期集合
            List<string> daysList = new List<string>();
            for (DateTime date = firstDayOfMonth; date < firstDayOfNextMonth; date = date.AddDays(1))
            {
                daysList.Add(date.ToString("yyyy-MM-dd"));
            }

            return daysList.ToArray();
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
                    CreateData(SelectedMonth);
                }
            }
        
        }
        public ChartValues<double> CalculateDailyMeetingDuration(List<Models.Meet> allMeetList,string month)
        {
            if (allMeetList.Count == 0)
            {
                DateTime defDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                int daysInMonth = DateTime.DaysInMonth(defDate.Year, defDate.Month);

                ChartValues<double> dailyMeetingDurationValues = new ChartValues<double>();
                for (int day = 1; day <= daysInMonth; day++)
                {
                    double duration = 0;

                    dailyMeetingDurationValues.Add(duration);
                }

                return dailyMeetingDurationValues;
            }
            else
            {
                // 初始化一个字典来存储每一天的会议总时长
                Dictionary<int, double> dailyMeetingDuration = new Dictionary<int, double>();

                // 遍历所有的会议
                foreach (var meet in allMeetList)
                {
                
                    // 获取会议的日期（年月日部分）
                    DateTime meetDate = meet.MeetStartTime.Date;

                    // 如果字典中不存在该日期，则添加并将时长设为当前会议的时长
                    if (!dailyMeetingDuration.ContainsKey(meetDate.Day))
                    {
                        dailyMeetingDuration[meetDate.Day] = (meet.MeetEndTime - meet.MeetStartTime).TotalHours;
                    }
                    else
                    {
                        // 如果已经存在该日期，则累加时长
                        dailyMeetingDuration[meetDate.Day] += (meet.MeetEndTime - meet.MeetStartTime).TotalHours;
                    }
                }
                // 获取传入数据中的最早日期和最晚日期
                DateTime startDate = allMeetList.Min(x => x.MeetStartTime.Date);
                DateTime endDate = allMeetList.Max(x => x.MeetEndTime.Date);
                int daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);

                // 生成每一天的会议时长，如果某一天没有会议，则插入0作为占位
                ChartValues<double> dailyMeetingDurationValues = new ChartValues<double>();
                for (int day = 1; day <= daysInMonth; day++)
                {
                    double duration = 0;

                    if (dailyMeetingDuration.ContainsKey(day))
                    {
                        duration = dailyMeetingDuration[day];
                    }

                    dailyMeetingDurationValues.Add(duration);
                }

                return dailyMeetingDurationValues;
            }
           
        }
        public void CreateData(string month)
        {
            string[] monthToDayList = GenerateDaysInMonth(month);
            var addMeets = new ApiRequest();
            addMeets.method = Method.GET;
            addMeets.route = "api/MeetControllers/FindAll";
            var MeetListsResponse = httpClients.Excute(addMeets);
            JArray  jArray = (JArray)MeetListsResponse.ResultData;
            List<Models.Meet> allMeetList  = jArray.ToObject<List<Models.Meet>>();
            List<Models.Meet> meetAList = allMeetList.Where(x => x.MeetingRoom == "502A" &&  x.MeetStartTime.ToString("yyyy-MM").StartsWith(month)).ToList();
            List<Models.Meet> meetBList = allMeetList.Where(x => x.MeetingRoom == "502B TV" &&  x.MeetStartTime.ToString("yyyy-MM").StartsWith(month)).ToList();
            List<Models.Meet> meetCList = allMeetList.Where(x => x.MeetingRoom == "502C" &&  x.MeetStartTime.ToString("yyyy-MM").StartsWith(month)).ToList();
            Application.Current.Dispatcher.Invoke(() =>
            {
                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "502A",
                        Values = CalculateDailyMeetingDuration(meetAList,month)
                    },
                    new LineSeries
                    {
                        Title = "502B TV",
                        Values = CalculateDailyMeetingDuration(meetBList,month)
                    },
                    new LineSeries
                    {
                        Title = "502C",
                        Values = CalculateDailyMeetingDuration(meetCList,month)
                    },
                };
                Labels = monthToDayList;
                YFormatter =  value => TimeSpan.FromHours(value).ToString("hh\\:mm");
                OnPropertyChanged(nameof(SeriesCollection));
                OnPropertyChanged(nameof(Labels));
                OnPropertyChanged(nameof(YFormatter));
            });
            
            // Labels = new[] {"1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"};
            // YFormatter = value => new DateTime((long)value).ToString("dd MMM yyyy HH:mm");
            // YFormatter = value => value.ToString("C");
            
        }
        private string[] _labels;
        public string[] Labels
        {
            get => _labels;
            set
            {
                if (_labels != value)
                {
                    _labels = value;
                    OnPropertyChanged(nameof(_labels));
                }
            }
        }
        private Func<double, string> _yFormatter;
        public Func<double, string> YFormatter
        {
            get => _yFormatter;
            set
            {
                if (_yFormatter != value)
                {
                    _yFormatter = value;
                    OnPropertyChanged(nameof(_yFormatter));
                }
            }
        }
        //图表对象
         private SeriesCollection _seriesCollection;
         public SeriesCollection SeriesCollection
         {
             get => _seriesCollection;
             set
             {
                 if (_seriesCollection != value)
                 {
                     _seriesCollection = value;
                     OnPropertyChanged(nameof(_seriesCollection));
                 }
             }
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
};

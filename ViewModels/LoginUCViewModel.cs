using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Meet.HttpClients;
using MeetWebApiPro.DataModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestSharp;

namespace Meet.ViewModels;

internal class LoginUCViewModel : BindableBase, IDialogAware
{
    public readonly HttpRestClient httpClients;

    //注册对象双向绑定
    private AccountInfo _AccountInfo;


    //下列列表
    private ObservableCollection<string> _addressList;

    //登陆账号和密码
    private string _LoginGmail;

    //密码定义

    private string _selectedAddress;

    //显示内容的下标
    private int _SelectedIndex;

    public LoginUCViewModel(HttpRestClient _httpClients)
    {
        // 初始化数据源
        AddressList = new ObservableCollection<string>
        {
            "武汉",
            "大连",
            "北京"
        };
        //登陆监听入口
        LoginCmm = new DelegateCommand(Login);
        //注册页面监听入口
        ShowRegInfoCmm = new DelegateCommand(ShowRegInfo);
        BackRegInfoCmm = new DelegateCommand(BackRegInfo);
        ReqCmm = new DelegateCommand(ReqCmmCnm);
        AccountInfo = new AccountInfo();
        httpClients = _httpClients;
    }

    public DelegateCommand LoginCmm { get; set; }

    public string Pwd { get; set; }

    public string LoginGmail
    {
        get => _LoginGmail;
        set
        {
            _LoginGmail = value;
            RaisePropertyChanged();
        }
    }

    public AccountInfo AccountInfo
    {
        get => _AccountInfo;
        set
        {
            _AccountInfo = value;
            RaisePropertyChanged();
        }
    }

    //注册按钮绑定
    public DelegateCommand ReqCmm { get; set; }

    //下列列表
    public ObservableCollection<string> AddressList
    {
        get => _addressList;
        set => SetProperty(ref _addressList, value);
    }

    //下列列表
    public string SelectedAddress
    {
        get => _selectedAddress;
        set => SetProperty(ref _selectedAddress, value);
    }

    //显示内容的下标,绑定前端数据,学习下双向绑定哈哈
    public int SelectedIndex
    {
        get => _SelectedIndex;
        set
        {
            _SelectedIndex = value;
            RaisePropertyChanged();
        }
    }

    //调用显示注册界面的函数
    public DelegateCommand ShowRegInfoCmm { get; set; }

    //调用退出注册界面的函数
    public DelegateCommand BackRegInfoCmm { get; set; }
    public string Title { get; set; } = "会议系统";

    public event Action<IDialogResult> RequestClose;

    //
    public bool CanCloseDialog()
    {
        return true;
    }

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
    }

    public static AccountInfo AccountInfoMsg = new AccountInfo(1,"mika","123456","12sd2q","2824485612@qq.com");
    private void Login()
    {
        var api = new ApiRequest();
        api.method = Method.GET;
        api.route = "api/Account/Login?user=" + LoginGmail + "&password=" + Pwd;
        var response = httpClients.Excute(api);
        AccountInfoMsg =(AccountInfo) response.ResultData;
        if (response.ResultCode == 200)
        {
            if (RequestClose != null)
            {
                RequestClose(new DialogResult(ButtonResult.OK));
                MessageBox.Show("登陆成功");
            }
            else
            {
                MessageBox.Show("登陆成功");
            }
        }
        else
        {
            MessageBox.Show("登陆失败");
        }
    }

    //显示注册注册界面的函数
    private void ReqCmmCnm()
    {
        var api = new ApiRequest();
        api.method = Method.GET;
        api.route = "api/Account/Login?user=" + AccountInfo.Name + "&password=" + AccountInfo.Password;
        var response = httpClients.Excute(api);
        Debug.WriteLine("response注册" + response);
        Debug.WriteLine("注册");
    }

    //显示注册界面的函数
    private void ShowRegInfo()
    {
        SelectedIndex = 1;
    }

    //显示退出注册界面的函数
    private void BackRegInfo()
    {
        SelectedIndex = 0;
    }
}
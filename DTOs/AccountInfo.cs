namespace MeetWebApiPro.DataModel;

public class AccountInfo
{
    public AccountInfo()
    {
    }

    public AccountInfo(int accountId, string name, string password, string regPassword, string dxcGmail)
    {
        AccountId = accountId;
        Name = name;
        Password = password;
        RegPassword = regPassword;
        DxcGmail = dxcGmail;
    }

    public int AccountId { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string RegPassword { get; set; }

    public string DxcGmail { get; set; }
}